import { createAction, createSlice, PayloadAction } from '@reduxjs/toolkit';
import { call, put, takeEvery } from 'redux-saga/effects';
import { RootState } from '../../app/store';
import { AppState } from './appModels';
import { getEthBlockNumber } from './appService';

const initialState: AppState = { ethBlockNumber: 0, ethProviderUrl: null };

export const appSlice = createSlice({
  name: 'app',
  initialState,
  reducers: {
    setEthProviderUrl: (state, action: PayloadAction<string>) => {
      return { ...state, ethProviderUrl: action.payload };
    },
    setEthBlockNumber: (state, action: PayloadAction<number>) => {
      return { ...state, ethBlockNumber: action.payload };
    },
  },
});

export const selectApp = (state: RootState) => state.app;

export default appSlice.reducer;

export const { setEthProviderUrl, setEthBlockNumber } = appSlice.actions;

export function* appSaga() {}
