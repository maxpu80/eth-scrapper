import type { AppProps } from 'next/app';
import { useEffect } from 'react';
import { Provider } from 'react-redux';
import { store } from '../app/store';
import { rehydrateConfigRequest } from '../features/app/appSlice';
import { fetchAllRequest } from '../features/projects/projectsSlice';
import '../styles/globals.css';

function MyApp({ Component, pageProps }: AppProps) {
  useEffect(() => {
    store.dispatch(fetchAllRequest());
    store.dispatch(rehydrateConfigRequest());
  });

  return (
    <Provider store={store}>
      <Component {...pageProps} />
    </Provider>
  );
}

export default MyApp;
