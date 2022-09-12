import { configureStore } from '@reduxjs/toolkit';
import projectsReducer, { projectsSaga } from '../features/projects/projectsSlice';
import createSagaMiddleware from '@redux-saga/core';
import appReducer, { appSaga } from '../features/app/appSlice';

const sagaMiddleware = createSagaMiddleware();

export const store = configureStore({
  reducer: {
    projects: projectsReducer,
    app: appReducer,
  },
  middleware: [sagaMiddleware],
});

export function* mainSaga() {
  yield appSaga();
  yield projectsSaga();  
}

sagaMiddleware.run(mainSaga);

export type RootState = ReturnType<typeof store.getState>;

export type AppDispatch = typeof store.dispatch;
