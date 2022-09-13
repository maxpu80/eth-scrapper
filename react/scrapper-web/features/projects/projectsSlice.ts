import { createAction, createSlice, PayloadAction } from '@reduxjs/toolkit';
import { omit, set } from 'lodash';
import { eventChannel } from 'redux-saga';
import { call, put, takeEvery } from 'redux-saga/effects';
import { RootState } from '../../app/store';
import { ApiResult } from '../sharedModels';
import { Project, ProjectState, ScrapperState, VersionAction } from './projectModels';
import { getProjects } from './projectsService';

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
    stateChanges: (state, action: PayloadAction<ProjectState>) => {
      return action.payload;
    },
  },
});

export const selectProjects = (state: RootState) => state.projects;

export const { add, remove, setVersionState } = projectsSlice.actions;

export default projectsSlice.reducer;

//
export const fetchAllRequest = createAction('projects/fetchAllRequest');

function* fetchAll() {
  const projects: ApiResult<Project[]> = yield call(getProjects);
  if (projects.kind === 'ok') {
    const data = Object.fromEntries(projects.value.map((x) => [x.id, x]));
    yield put(projectsSlice.actions.fetchAllSuccess(data));
  }
}

//
export const startQueryStateChanges = createAction<number>('projects/startQueryStateChanges');

function queryStateChangesChannel(interval: number) {
  return eventChannel((emitter) => {
    const iv = setInterval(() => {
      emitter(true);
    }, interval);
    // The subscriber must return an unsubscribe function
    return () => {
      clearInterval(iv);
    };
  });
}

export function* projectsSaga() {
  yield takeEvery(fetchAllRequest.toString(), fetchAll);
  //@ts-ignore
  const stateChangesChannel: any = yield call(queryStateChangesChannel, 1000 * 10);
  yield takeEvery(stateChangesChannel, function* () {
    yield;
    const projects: ApiResult<Project[]> = yield call(getProjects);
    if (projects.kind === 'ok') {
      const data = Object.fromEntries(projects.value.map((x) => [x.id, x]));
      yield put(projectsSlice.actions.stateChanges(data));
    }
  });
}
