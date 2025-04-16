'use client';

import AnimationQrCode from '../public/AnimationQrCode.json';
import Lottie from 'react-lottie-player';
import { getCookie, deleteCookie } from 'cookies-next';
import { useState } from 'react';
import { useAppDispatch } from '@/hooks/reduxHooks';
import { resetPersonnelData } from '@/store/personnelSlice';

const QrCode = () => {
    const [token, setToken] = useState<any>(getCookie('access-token'));
    const dispatch = useAppDispatch();

    const handleLogOut = () => {
        deleteCookie('access-token');
        deleteCookie('refresh-token');
        dispatch(resetPersonnelData());
        setToken(null);
        window.location.reload();
    };

    return (
        <div className="w-[260px] h-[260px]">
            <Lottie loop animationData={AnimationQrCode} play />

            <p className="text-center font-medium text-base">
                © 2025 All rights reserved
            </p>

            {token && (
                <button
                    onClick={handleLogOut}
                    className="flex mx-auto mt-12 bg-black shadow-md px-10 py-3 rounded-md font-medium text-white hover:bg-orange-800 transition-all"
                >
                    Log Out
                </button>
            )}
        </div>
    );
};

export default QrCode;
