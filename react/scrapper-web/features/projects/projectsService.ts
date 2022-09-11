import { CreateProjectError, Project } from './projectModels';

export interface AddProjectData {
  contractAddress: string;
}

export interface Ok<T> {
  kind: 'ok';
  value: T;
}

export interface Error<T> {
  kind: 'error';
  error: T;
}

export type Result<S, E> = Ok<S> | Error<E>;

export const createProject = async (data: AddProjectData): Promise<Result<Project, CreateProjectError>> => {
  try {
    const contractAddress = data.contractAddress;
    const abiUrl = `https://api.etherscan.io/api?module=contract&action=getabi&address=${contractAddress}`;
    const abiResult = await fetch(abiUrl);
    const abiJsonResult = await abiResult.json();
    if (abiJsonResult.code === '0') {
      return { kind: 'error', error: 'get-abi-error' };
    }
    const abi = abiJsonResult.result;
    const headers = {
      'dapr-app-id': 'projects-api',
      'Content-Type': 'application/json',
      Accept: 'application/json',
    };
    const body = {
      id: data.contractAddress,
      contractAddress: data.contractAddress,
      name: data.contractAddress,
      abi,
    };
    const strBody = JSON.stringify(body);
    const postResult = await fetch('http://localhost:6001/projects', { method: 'post', headers, body: strBody });
    console.log('!!!', postResult);
    const jsonResult = await postResult.json();
    return { kind: 'ok', value: jsonResult };
  } catch (err) {
    console.error(err);
    return { kind: 'error', error: 'api-error' };
  }
};
