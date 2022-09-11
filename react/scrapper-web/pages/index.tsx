import type { NextPage } from 'next';
import { useAppDispatch, useAppSelector } from '../app/hooks';
import AddProject, { AddProjectData } from '../components/AddProject';
import ProjectsList from '../components/ProjectsList';
import { add, fetchAllRequest, selectProjects } from '../features/projects/projectsSlice';

const Projects: NextPage = () => {
  const projects = useAppSelector(selectProjects);
  const dispatch = useAppDispatch();

  const onAdd = async (data: AddProjectData) => {
    console.log('!!!!', data);
    const id = new Date().getTime().toString();
    dispatch(add({ id, name: data.contractAddress }));
    return 'ok' as 'ok';
  };

  return (
    <>
      <AddProject onAdd={onAdd}></AddProject>
      <ProjectsList projects={projects}></ProjectsList>
    </>
  );
};

export default Projects;
