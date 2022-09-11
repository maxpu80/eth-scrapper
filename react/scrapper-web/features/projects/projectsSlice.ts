import { createSlice } from '@reduxjs/toolkit';

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
  reducers: {},
});

export default projectsSlice.reducer;