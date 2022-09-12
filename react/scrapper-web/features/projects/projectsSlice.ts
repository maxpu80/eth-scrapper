import { createAction, createSlice, PayloadAction } from '@reduxjs/toolkit';
import { RootState } from '../../app/store';
import { put, takeEvery, call } from 'redux-saga/effects';
import { Project, ProjectState, ScrapperState, ScrapperVersion, VersionAction } from './projectModels';
import { getProjects } from './projectsService';
import Projects from '../../pages';
import { ApiResult } from '../sharedModels';
import { omit, set } from 'lodash';

export interface AddActionPayload {
  id: string;
  name: string;
}

export interface VersionActionPayload {
  projectId: string;
  versionId: string;
  action: VersionAction;
  state: ScrapperState;
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
    setVersionState: (state, action: PayloadAction<VersionActionPayload>) => {
      return set(
        state,
        [action.payload.projectId, 'versions', action.payload.versionId, 'state'],
        action.payload.state,
      );
    },
  },
});

export const selectProjects = (state: RootState) => state.projects;

export const { add, remove, setVersionState } = projectsSlice.actions;

export default projectsSlice.reducer;

export const fetchAllRequest = createAction('projects/fetchAllRequest');

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
