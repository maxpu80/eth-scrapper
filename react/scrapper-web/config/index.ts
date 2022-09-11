import { DataAccessConfig } from '../features/dataAcess';

const appConfig = {
  api: {
    baseUrl: 'http://localhost:6001',
    daprAppId: 'scrapper-api',
  } as DataAccessConfig,
};

export default appConfig;
