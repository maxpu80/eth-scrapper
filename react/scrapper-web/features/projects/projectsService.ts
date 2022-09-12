import axios from 'axios';
import { dataAccess } from '../dataAcess';
import { ApiError, ApiResult } from '../sharedModels';
import { CreateProjectError, CreateProjectResult, Project, ScrapperState, VersionAction } from './projectModels';
export interface AddProjectData {
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
  // const abiResult = await getAbi(data.contractAddress);
  // if (abiResult.kind === 'error') {
  //   return abiResult;
  // } else {
  //   const body = {
  //     id: data.contractAddress,
  //     contractAddress: data.contractAddress,
  //     name: data.contractAddress,
  //     abi: abiResult.value,
  //   } as Project;
  //   return dataAccess.post<Project>('projects', body);

  // }

  const body = {
    id: data.contractAddress,
    contractAddress: data.contractAddress,
    name: data.contractAddress,
    abi: '',
    versions: {
      v1: {
        id: 'v1',
        name: 'v1',
        createdAt: new Date().getTime(),
      },
    },
  } as Project;
  return { kind: 'ok', value: body };
};

export const getProjects = async (): Promise<ApiResult<Project[]>> => {
  //return dataAccess.get<Project[]>('projects');
  return {
    kind: 'ok',
    value: [
      {
        id: 'test',
        name: 'test1',
        contractAddress: 'test',
        abi: 'test',
        versions: {
          v1: {
            id: 'v1',
            name: 'v1',
            createdAt: new Date().getTime(),
            state: {
              updatedAt: new Date().getTime(),
              status: 'pause',
              requestBlock: {},
            },
          },
        },
      },
    ],
  };
};

export const removeProject = async (id: string): Promise<ApiResult<Project>> => {
  //return dataAccess.remove<Project>(`projects/${id}`);
  return {
    kind: 'ok',
    value: {
      id,
      name: 'test1',
      contractAddress: 'test',
      abi: 'test',
      versions: {},
    },
  };
};

export const projectVersionAction = async (
  projectId: string,
  versionId: string,
  action: VersionAction,
): Promise<ApiResult<ScrapperState>> => {
  //return dataAccess.post<ScrapperState>(`projects/${projectId}/versions/${versionId}/${action}`, {});
  return {
    kind: 'ok',
    value: {
      updatedAt: new Date().getTime(),
      status: 'continue',
      requestBlock: {},
    },
  };
};
