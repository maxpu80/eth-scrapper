import { DataAccessConfig } from '../features/dataAcess';

const appConfig = {
  api: {
    baseUrl: 'http://localhost:6001',
    daprAppId: 'scrapper-api',
  } as DataAccessConfig,
  stateChangesQueryInterval: 60 * 1000,
};

export default appConfig;
