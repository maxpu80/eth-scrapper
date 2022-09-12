import { ProjectState, VersionAction } from '../features/projects/projectModels';
import { VersionsList } from './VersionsList';

export interface ProjectsListProps {
  projects: ProjectState;
  onRemove: (id: string) => void;
  onVersionAction: (projectId: string, versionId: string, action: VersionAction) => void;
  ethBlockNumber: number;
}

const ProjectsList = ({ projects, onRemove, onVersionAction, ethBlockNumber }: ProjectsListProps) => {
  return (
    <ul>
      {Object.values(projects).map((project) => (
        <li key={project.id}>
          {project.name} <a onClick={() => onRemove(project.id)}>remove</a>
          <VersionsList
            onAction={(versionId, action) => onVersionAction(project.id, versionId, action)}
            versions={project.versions}
            ethBlockNumber={ethBlockNumber}></VersionsList>
        </li>
      ))}
    </ul>
  );
};

export default ProjectsList;
