import { ApiError, ApiResult } from '../sharedModels';

export type ScrapperStatus = 'continue' | 'pause' | 'finish' | 'schedule';

export interface ScrapperRequestBlock {
  from?: number;
  to?: number;
}

export interface ScrapperState {
  updatedAt: number;
  status: ScrapperStatus;
  block: ScrapperRequestBlock;
  requestBlock: ScrapperRequestBlock;
}

export interface  ScrapperVersion {
  id: string;
  name: string;
  createdAt: number;
  state?: ScrapperState;
}

export interface ScrapperVersionMap {
  [key: string]: ScrapperVersion;
}

export interface Project {
  id: string;
  contractAddress: string;
  name: string;
  abi: string;
  versions: ScrapperVersionMap;
}

export type CreateProjectError = ApiError | { kind: 'get-abi-error' };

export type CreateProjectResult = ApiResult<Project, CreateProjectError>;

export interface ProjectState {
  [key: string]: Project;
}

export type VersionAction = 'start' | 'pause' | 'resume' | 'reset';