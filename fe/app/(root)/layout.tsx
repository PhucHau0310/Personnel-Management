'use client';

import Layout from '@/components/Layout';
import { Provider } from 'react-redux';
import { PersistGate } from 'redux-persist/integration/react';
import { store, persistor } from '../../store/store';
import useAlert from '../../hooks/useAlert';
import { useEffect } from 'react';
import { setupAxiosInterceptors } from '@/helper/axiosJwt';

const ManagementLayout = ({ children }: { children: React.ReactNode }) => {
    const { showAlert, AlertComponent } = useAlert();

    useEffect(() => {
        setupAxiosInterceptors(showAlert);
    }, []);

    return (
        <>
            {AlertComponent}
            <Provider store={store}>
                <PersistGate loading={null} persistor={persistor}>
                    <div>
                        <Layout children={children} />
                    </div>
                </PersistGate>
            </Provider>
        </>
    );
};

export default ManagementLayout;
