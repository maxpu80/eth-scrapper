import { createAction, createSlice, PayloadAction } from '@reduxjs/toolkit';
import { set } from 'lodash';
import { call, put, takeEvery } from 'redux-saga/effects';
import { RootState } from '../../app/store';
import { Result } from '../sharedModels';
import { AppConfig, AppState } from './appModels';
import { getEthBlockNumber, getEthProviderUrl } from './appService';

const initialState: AppState = { ethBlockNumber: 0, config: { ethProviderUrl: null } };

export const appSlice = createSlice({
  name: 'app',
  initialState,
  reducers: {
    setEthProviderUrl: (state, action: PayloadAction<string>) => {
      return set(state, ['config', 'ethProviderUrl'], action.payload);
    },
    setEthBlockNumber: (state, action: PayloadAction<number>) => {
      return { ...state, ethBlockNumber: action.payload };
    },
    setConfig: (state, action: PayloadAction<AppConfig>) => {
      return { ...state, config: action.payload };
    },
  },
});

export const selectApp = (state: RootState) => state.app;

export default appSlice.reducer;

export const { setEthProviderUrl, setEthBlockNumber } = appSlice.actions;

export const rehydrateConfigRequest = createAction('app/rehydrateConfigRequest');

function* rehydrateConfig() {
  const ethProviderUrl: string = yield call(getEthProviderUrl);
  if (ethProviderUrl !== null) {
    yield put(appSlice.actions.setConfig({ ethProviderUrl }));
    const ethBlockNumber: Result<number, string> = yield call(getEthBlockNumber, ethProviderUrl);
    if (ethBlockNumber.kind === 'ok') {
      yield put(setEthBlockNumber(ethBlockNumber.value));
    }
  }
}

export function* appSaga() {
  yield takeEvery(rehydrateConfigRequest.toString(), rehydrateConfig);
}
