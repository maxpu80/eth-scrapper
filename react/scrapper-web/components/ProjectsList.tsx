import { ProjectState } from '../features/projects/projectsSlice';

export interface ProjectsListProps {
  projects: ProjectState;
}

const ProjectsList = ({ projects }: ProjectsListProps) => {
  return (
    <ul>
      {Object.values(projects).map((project) => (
        <li key={project.id}>{project.name}</li>
      ))}
    </ul>
  );
};

export default ProjectsList;
