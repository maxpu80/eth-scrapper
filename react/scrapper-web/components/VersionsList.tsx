import { ScrapperState, ScrapperVersionMap, VersionAction } from '../features/projects/projectModels';
import { VersionActions } from './VersionActions';

export interface VersionsListProps {
  versions: ScrapperVersionMap;
  onAction: (id: string, action: VersionAction) => void;
}

export const VersionsList = ({ versions, onAction }: VersionsListProps) => {
  const renderProgress = (state?: ScrapperState) => {
    if (state) {
      return (
        <>
          <br />
          <small>progress: {state?.block.to} blocks of --- ~ 10 % </small>
        </>
      );
    } else {
      return <></>;
    }
  };
  return (
    <ul>
      {Object.values(versions).map((x) => (
        <li key={x.id}>
          {x.name}
          <VersionActions
            state={x.state}
            onAction={(action) => onAction(x.id, action)}></VersionActions>
          <br />
          <small>
            state: {x.state ? x.state.status : 'not started'} last updated{' '}
            {x.state ? new Date(x.state.updatedAt).toISOString() : '---'}
          </small>
          {renderProgress(x.state)}
        </li>
      ))}
    </ul>
  );
};
