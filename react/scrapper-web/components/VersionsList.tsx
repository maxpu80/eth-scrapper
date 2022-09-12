import { ScrapperState, ScrapperVersionMap, VersionAction } from '../features/projects/projectModels';
import { VersionActions } from './VersionActions';

export interface VersionsListProps {
  versions: ScrapperVersionMap;
  onAction: (id: string, action: VersionAction) => void;
  ethBlockNumber: number;
}

export const VersionsList = ({ versions, onAction, ethBlockNumber }: VersionsListProps) => {
  const renderProgress = (state?: ScrapperState) => {
    if (state && ethBlockNumber) {
      return (
        <>
          <br />
          <small>
            progress: {state!.requestBlock.from || 0} blocks of {ethBlockNumber} ~
            {Math.floor((state!.requestBlock.from || 0) / ethBlockNumber) * 100} %
          </small>
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
          {x.id}
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
