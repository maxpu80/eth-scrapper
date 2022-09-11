import type { NextPage } from 'next';
import { useAppDispatch, useAppSelector } from '../app/hooks';
import AddProject from '../components/AddProject';
import ProjectsList from '../components/ProjectsList';
import { AddProjectData, createProject, removeProject } from '../features/projects/projectsService';
import { add, remove, selectProjects } from '../features/projects/projectsSlice';

const Projects: NextPage = () => {
  const projects = useAppSelector(selectProjects);

  const dispatch = useAppDispatch();

  const onAdd = async (data: AddProjectData) => {
    const result = await createProject(data);
    switch (result.kind) {
      case 'ok':
        dispatch(add(result.value));
        return result;
      case 'error':
        return result;
    }
  };

  const onRemove = async (id: string) => {
    const result = await removeProject(id);
    switch (result.kind) {
      case 'ok':
        dispatch(remove(id));
        return result;
      case 'error':
        return result;
    }
  };

  return (
    <>
      <AddProject onAdd={onAdd}></AddProject>
      <ProjectsList
        projects={projects}
        onRemove={onRemove}></ProjectsList>
    </>
  );
};

export default Projects;
