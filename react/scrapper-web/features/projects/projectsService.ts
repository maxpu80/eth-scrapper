import axios from 'axios';
import { dataAccess } from '../dataAcess';
import { ApiError, ApiResult } from '../sharedModels';
import {
  CreateProjectError,
  CreateProjectResult,
  Project,
  ScrapperState,
  ScrapperVersion,
  VersionAction,
} from './projectModels';
export interface AddProjectData {
  ethProviderUrl: string;
  contractAddress: string;
}

const getAbi = async (contractAddress: string): Promise<ApiResult<string, CreateProjectError>> => {
  const abiUrl = `https://api.etherscan.io/api?module=contract&action=getabi&address=${contractAddress}`;
  const abiResult = await axios.get(abiUrl);
  const abiJsonResult = abiResult.data;
  if (abiJsonResult.status === '0') {
    return { kind: 'error', error: { kind: 'get-abi-error' } };
  } else {
    const abi = abiJsonResult.result;
    return { kind: 'ok', value: abi };
  }
};

export const createProject = async (data: AddProjectData): Promise<CreateProjectResult> => {
  const abiResult = await getAbi(data.contractAddress);
  if (abiResult.kind === 'error') {
    return abiResult;
  } else {
    const address = data.contractAddress.toLowerCase();
    const body = {
      id: address,
      address: address,
      name: address,
      abi: abiResult.value,
      ethProviderUrl: data.ethProviderUrl,
      versionId: 'v1',
    } as Partial<Project>;
    const projectResult = await dataAccess.post<Project & { versions: ScrapperVersion[] }>('projects', body);
    if (projectResult.kind === 'ok') {
      return {
        kind: 'ok',
        value: {
          ...projectResult.value,
          versions: Object.fromEntries(projectResult.value.versions.map((x) => [x.id, x])),
        },
      };
    } else {
      return projectResult;
    }
  }

  // const body = {
  //   id: data.contractAddress,
  //   address: data.contractAddress,
  //   name: data.contractAddress,
  //   abi: '',
  //   ethProviderUrl: 'xxx',
  //   versions: {
  //     v1: {
  //       id: 'v1',
  //       name: 'v1',
  //       createdAt: new Date().getTime(),
  //     },
  //   },
  // } as Project;
  // return { kind: 'ok', value: body };
};

export const getProjects = async (): Promise<ApiResult<Project[]>> => {
  const result = await dataAccess.get<
    { project: Project; versions: { version: ScrapperVersion; state: ScrapperState }[] }[]
  >('projects');
  if (result.kind === 'ok') {
    const projects = result.value.map((x) => ({
      ...x.project,
      versions: Object.fromEntries(x.versions.map((e) => [e.version.id, { ...e.version, state: e.state }])),
    }));
    return { kind: 'ok', value: projects };
  } else {
    return result;
  }
  // return {
  //   kind: 'ok',
  //   value: [
  //     {
  //       id: 'test',
  //       name: 'test1',
  //       address: 'test',
  //       abi: 'test',
  //       ethProviderUrl: 'xxx',
  //       versions: {
  //         v1: {
  //           id: 'v1',
  //           name: 'v1',
  //           createdAt: new Date().getTime(),
  //           state: {
  //             updatedAt: new Date().getTime(),
  //             status: 'pause',
  //             requestBlock: {},
  //           },
  //         },
  //       },
  //     },
  //   ],
  // };
};

export const removeProject = async (id: string): Promise<ApiResult<Project>> => {
  return dataAccess.remove<Project>(`projects/${id}`);
  // return {
  //   kind: 'ok',
  //   value: {
  //     id,
  //     name: 'test1',
  //     address: 'test',
  //     abi: 'test',
  //     versions: {},
  //     ethProviderUrl: 'xxx',
  //   },
  // };
};

export const projectVersionAction = async (
  projectId: string,
  versionId: string,
  action: VersionAction,
): Promise<ApiResult<ScrapperState>> => {
  return dataAccess.post<ScrapperState>(`projects/${projectId}/versions/${versionId}/${action}`, {});
  // return {
  //   kind: 'ok',
  //   value: {
  //     updatedAt: new Date().getTime(),
  //     status: 'continue',
  //     requestBlock: {},
  //   },
  // };
};
