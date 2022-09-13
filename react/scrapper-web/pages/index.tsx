import type { NextPage } from 'next';
import { useEffect } from 'react';
import { useAppDispatch, useAppSelector } from '../app/hooks';
import AddProject from '../components/AddProject';
import { AppConfig } from '../components/AppConfig';
import ProjectsList from '../components/ProjectsList';
import { getEthBlockNumber, storeEthProviderUrl } from '../features/app/appService';
import { rehydrateConfigRequest, selectApp, setEthBlockNumber, setEthProviderUrl } from '../features/app/appSlice';
import { VersionAction } from '../features/projects/projectModels';
import {
  AddProjectData,
  createProject,
  projectVersionAction,
  removeProject,
} from '../features/projects/projectsService';
import { add, fetchAllRequest, remove, selectProjects, setVersionState } from '../features/projects/projectsSlice';

let started = false;

const Projects: NextPage = () => {
  const projects = useAppSelector(selectProjects);
  const app = useAppSelector(selectApp);

  const dispatch = useAppDispatch();

  useEffect(() => {
    if (!started) {
      dispatch(fetchAllRequest());
      dispatch(rehydrateConfigRequest());
      setInterval(() => console.log('interval'), 5000);
    }
    started = true;
  });

  const onAdd = async (data: AddProjectData) => {
    // TODO: ethProviderUrl must be set when project created
    const result = await createProject({ ...data, ethProviderUrl: app.config.ethProviderUrl! });
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

  const onVersionAction = async (projectId: string, versionId: string, action: VersionAction) => {
    const result = await projectVersionAction(projectId, versionId, action);
    switch (result.kind) {
      case 'ok':
        dispatch(setVersionState({ projectId, versionId, state: result.value, action }));
        return result;
      case 'error':
        return result;
    }
  };

  const onSetProviderUrl = async (url: string) => {
    const blockNumberResult = await getEthBlockNumber(url);
    if (blockNumberResult.kind === 'ok') {
      await storeEthProviderUrl(url);
      dispatch(setEthProviderUrl(url));
      dispatch(setEthBlockNumber(blockNumberResult.value));
    }
    return blockNumberResult;
  };

  return (
    <>
      <AppConfig onSetProviderUrl={onSetProviderUrl}></AppConfig>
      {app.config.ethProviderUrl ? (
        <>
          <AddProject onAdd={onAdd}></AddProject>
          <ProjectsList
            projects={projects}
            onVersionAction={onVersionAction}
            onRemove={onRemove}
            ethBlockNumber={app.ethBlockNumber}></ProjectsList>
        </>
      ) : (
        <></>
      )}
    </>
  );
};

export default Projects;
