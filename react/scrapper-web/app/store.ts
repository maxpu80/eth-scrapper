import { configureStore } from '@reduxjs/toolkit';
import projectsReducer, { projectsSaga } from '../features/projects/projectsSlice';
import createSagaMiddleware from '@redux-saga/core';

const sagaMiddleware = createSagaMiddleware();

export const store = configureStore({
  reducer: {
    projects: projectsReducer,
  },
  middleware: [sagaMiddleware],
});

sagaMiddleware.run(projectsSaga);

export type RootState = ReturnType<typeof store.getState>;

export type AppDispatch = typeof store.dispatch;
