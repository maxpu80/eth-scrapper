import type { NextPage } from 'next';
import { useAppDispatch, useAppSelector } from '../app/hooks';
import AddProject from '../components/AddProject';
import ProjectsList from '../components/ProjectsList';
import { AddProjectData, createProject } from '../features/projects/projectsService';
import { add, fetchAllRequest, selectProjects } from '../features/projects/projectsSlice';

const Projects: NextPage = () => {
  const projects = useAppSelector(selectProjects);
  const dispatch = useAppDispatch();

  const onAdd = async (data: AddProjectData) => {
    const result = await createProject(data);
    switch (result.kind) {
      case 'ok':
        dispatch(add(result.value));
        return 'ok' as 'ok';
      case 'error':
        return result.error;
    }
  };

  return (
    <>
      <AddProject onAdd={onAdd}></AddProject>
      <ProjectsList projects={projects}></ProjectsList>
    </>
  );
};

export default Projects;
