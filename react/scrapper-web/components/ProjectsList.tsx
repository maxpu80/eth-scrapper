import { ProjectState } from '../features/projects/projectsSlice';

export interface ProjectsListProps {
  projects: ProjectState;
  onRemove: (id: string) => void;
}

const ProjectsList = ({ projects, onRemove }: ProjectsListProps) => {
  return (
    <ul>
      {Object.values(projects).map((project) => (
        <li key={project.id}>
          {project.name} <a onClick={() => onRemove(project.id)}>remove</a>
        </li>
      ))}
    </ul>
  );
};

export default ProjectsList;
