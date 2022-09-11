import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { RootState } from '../../app/store';

export interface AddActionPayload {
  id: string;
  name: string;
}

export interface Project {
  id: string;
  name: string;
}

export interface ProjectState {
  [key: string]: Project;
}

const initialState: ProjectState = {
  '0': { id: '0', name: 'lol' },
};

export const projectsSlice = createSlice({
  name: 'projects',
  initialState,
  reducers: {
    add: (state, action: PayloadAction<AddActionPayload>) => {
      return { ...state, [action.payload.id]: action.payload };
    },
  },
});

export const selectProjects = (state: RootState) => state.projects;

export const { add } = projectsSlice.actions;

export default projectsSlice.reducer;
