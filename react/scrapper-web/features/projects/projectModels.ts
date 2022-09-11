import { ApiError, ApiResult } from '../sharedModels';

export interface Project {
  id: string;
  contractAddress: string;
  name: string;
  abi: string;
}

export type CreateProjectError = ApiError | { kind: 'get-abi-error' };

export type CreateProjectResult = ApiResult<Project, CreateProjectError>;
