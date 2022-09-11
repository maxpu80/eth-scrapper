import { createAction, createSlice, PayloadAction } from '@reduxjs/toolkit';
import { RootState } from '../../app/store';
import { put, takeEvery, call } from 'redux-saga/effects';
import { Project } from './projectModels';
import { getProjects } from './projectsService';
import Projects from '../../pages';
import { ApiResult } from '../sharedModels';
import { omit } from 'lodash';

export interface AddActionPayload {
  id: string;
  name: string;
}

export interface ProjectState {
  [key: string]: Project;
}

const initialState: ProjectState = {};

export const projectsSlice = createSlice({
  name: 'projects',
  initialState,
  reducers: {
    fetchAllSuccess: (state, action: PayloadAction<ProjectState>) => {
      return action.payload;
    },
    add: (state, action: PayloadAction<Project>) => {
      return { ...state, [action.payload.id]: action.payload };
    },
    remove: (state, action: PayloadAction<string>) => {
      return omit(state, action.payload);
    },
  },
});

export const selectProjects = (state: RootState) => state.projects;

export const { add, remove } = projectsSlice.actions;

export const fetchAllRequest = createAction('projects/fetchAllRequest');

export default projectsSlice.reducer;

function* fetchAll() {
  const projects: ApiResult<Project[]> = yield call(getProjects);
  if (projects.kind === 'ok') {
    const data = Object.fromEntries(projects.value.map((x) => [x.id, x]));
    yield put(projectsSlice.actions.fetchAllSuccess(data));
  }
}

export function* projectsSaga() {
  yield takeEvery(fetchAllRequest.toString(), fetchAll);
}
