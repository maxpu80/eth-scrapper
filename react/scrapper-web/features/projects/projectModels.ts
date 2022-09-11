export interface Project {
  id: string;
  contractAddress: string;
  name: string;
  abi: string;
}

export type CreateProjectError = 'get-abi-error' | 'api-error';