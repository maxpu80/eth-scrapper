import type { NextPage } from 'next';
import { useAppDispatch, useAppSelector } from '../app/hooks';
import { add, fetchAllRequest, selectProjects } from '../features/projects/projectsSlice';

const Home: NextPage = () => {
  const projects = useAppSelector(selectProjects);
  const dispatch = useAppDispatch();

  const onAdd = () => {
    const id = new Date().getTime().toString();
    dispatch(add({ id, name: id }));
  };

  return (
    <>
      <button onClick={() => dispatch(fetchAllRequest())}>Fetch</button>
      <button onClick={onAdd}>Add</button>
      <ul>
        {Object.values(projects).map((project) => (
          <li key={project.id}>{project.name}</li>
        ))}
      </ul>
    </>
  );
};

export default Home;
