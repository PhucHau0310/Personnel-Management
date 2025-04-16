'use client';

import { Alert, Snackbar } from '@mui/material';
import { useEffect, useState } from 'react';

const useAlert = () => {
    const [open, setOpen] = useState(false);
    const [message, setMessage] = useState<string | null>(null);
    const [severity, setSeverity] = useState<
        'success' | 'info' | 'warning' | 'error'
    >('info');

    const showAlert = (
        msg: string,
        level: 'success' | 'info' | 'warning' | 'error' = 'info'
    ) => {
        setMessage(msg);
        setSeverity(level);
        setOpen(true);
    };

    const handleClose = () => {
        setOpen(false);
        setMessage(null);
    };

    useEffect(() => {
        let timer: NodeJS.Timeout;
        if (open) {
            timer = setTimeout(() => handleClose(), 3000);
        }
        return () => clearTimeout(timer);
    }, [open]);

    const AlertComponent = (
        <Snackbar
            open={open}
            autoHideDuration={3000}
            onClose={handleClose}
            anchorOrigin={{ vertical: 'top', horizontal: 'right' }}
            style={{ zIndex: 9999 }}
        >
            <Alert severity={severity} onClose={handleClose} variant="filled">
                {message}
            </Alert>
        </Snackbar>
    );
    return { showAlert, AlertComponent };
};

export default useAlert;
