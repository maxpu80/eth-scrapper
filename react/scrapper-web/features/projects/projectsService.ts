import axios from 'axios';
import { dataAccess } from '../dataAcess';
import { ApiError, ApiResult } from '../sharedModels';
import { CreateProjectResult, Project } from './projectModels';
export interface AddProjectData {
  contractAddress: string;
}

const getAbi = async (contractAddress: string): Promise<CreateProjectResult> => {
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

export const createProject = async (data: AddProjectData): Promise<ApiResult<Project, CreateProjectError>> => {
  const abiResult = await getAbi(data.contractAddress);
  if (abiResult.kind === 'error') {
    return abiResult;
  } else {
    const body = {
      id: data.contractAddress,
      contractAddress: data.contractAddress,
      name: data.contractAddress,
      abi: abiResult.value,
    };
    return dataAccess.post<Project>('projects', body);
  }
};
