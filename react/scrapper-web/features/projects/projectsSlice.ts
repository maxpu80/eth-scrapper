import { createAction, createSlice, PayloadAction } from '@reduxjs/toolkit';
import { RootState } from '../../app/store';
import { put, takeEvery } from 'redux-saga/effects';
import { Project } from './projectModels';

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
  },
});

export const selectProjects = (state: RootState) => state.projects;

export const { add } = projectsSlice.actions;

export const fetchAllRequest = createAction('projects/fetchAllRequest');

export default projectsSlice.reducer;

function* fetchAll() {
  const data: ProjectState = {};
  yield put(projectsSlice.actions.fetchAllSuccess(data));
}

export function* projectsSaga() {
  yield takeEvery(fetchAllRequest.toString(), fetchAll);
}
