import { DataAccessConfig } from '../features/dataAcess';

const appConfig = {
  api: {
    baseUrl: process.env.BASE_URL || 'http://localhost:6001',
    daprAppId: process.env.DAPR_APP_ID || 'scrapper-api',
  } as DataAccessConfig,
  stateChangesQueryInterval: (+(process.env.STATE_CHANGES_QUERY_INTERVAL || 0) || 60) * 1000,
};

export default appConfig;
