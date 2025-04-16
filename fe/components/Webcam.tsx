'use client';

import React, { useRef, useEffect, useState, useCallback } from 'react';
import { useAppDispatch } from '@/hooks/reduxHooks';
import { setPersonnelData } from '@/store/personnelSlice';
import { formatDateTime } from '@/utils/formatDateTime';
import useAlert from '@/hooks/useAlert';
import axiosJwt from '@/helper/axiosJwt';
import {
    Html5Qrcode,
    Html5QrcodeSupportedFormats,
    Html5QrcodeScannerState,
} from 'html5-qrcode';

interface WebcamProps {
    isActive: boolean;
}

const Webcam = ({ isActive }: WebcamProps) => {
    const containerRef = useRef<HTMLDivElement>(null);
    const html5QrCodeRef = useRef<Html5Qrcode | null>(null);
    const [scanning, setScanning] = useState(false);
    const [cameraActive, setCameraActive] = useState(false);
    const [isTransitioning, setIsTransitioning] = useState(false);
    const [noQrFound, setNoQrFound] = useState(false);
    const [isProcessed, setIsProcessed] = useState(false);
    const processingRef = useRef(false);
    const lastScanTimeRef = useRef(0);
    const dispatch = useAppDispatch();
    const { showAlert, AlertComponent } = useAlert();

    // Debounce function to prevent multiple rapid scans
    const debounce = (func: (...args: any[]) => void, delay: number) => {
        let timeoutId: NodeJS.Timeout;
        return (...args: any[]) => {
            if (timeoutId) {
                clearTimeout(timeoutId);
            }
            timeoutId = setTimeout(() => {
                func(...args);
            }, delay);
        };
    };

    // Khởi tạo máy quét
    useEffect(() => {
        if (!html5QrCodeRef.current && containerRef.current) {
            const qrScannerId = 'qr-scanner';
            if (!document.getElementById(qrScannerId)) {
                const qrScannerElement = document.createElement('div');
                qrScannerElement.id = qrScannerId;
                qrScannerElement.style.width = '100%';
                qrScannerElement.style.height = '100%';
                containerRef.current.appendChild(qrScannerElement);
            }
            html5QrCodeRef.current = new Html5Qrcode(qrScannerId, {
                verbose: false,
            });
        }

        return () => {
            if (html5QrCodeRef.current && cameraActive) {
                html5QrCodeRef.current
                    .stop()
                    .then(() => {
                        setCameraActive(false);
                        setIsTransitioning(false);
                    })
                    .catch((err) => {
                        console.error(
                            'Error stopping camera during cleanup:',
                            err
                        );
                    });
            }
        };
    }, []);

    // Hàm dừng máy quét
    const stopScanner = useCallback(async () => {
        if (!html5QrCodeRef.current || !cameraActive || isTransitioning) return;

        try {
            setIsTransitioning(true);
            await html5QrCodeRef.current.stop();
            setCameraActive(false);
            setIsTransitioning(false);
            setIsProcessed(false);
            processingRef.current = false;
        } catch (error) {
            console.error('Error stopping scanner:', error);
            setIsTransitioning(false);
        }
    }, [cameraActive, isTransitioning]);

    // Hàm khởi động máy quét
    const startScanner = useCallback(async () => {
        if (!html5QrCodeRef.current || cameraActive || isTransitioning) return;

        try {
            setIsTransitioning(true);
            setScanning(true);

            const currentState = html5QrCodeRef.current.getState();
            if (currentState === Html5QrcodeScannerState.SCANNING) {
                await html5QrCodeRef.current.stop();
            }

            const devices = await Html5Qrcode.getCameras();
            if (!devices || devices.length === 0) {
                throw new Error('No camera devices found');
            }

            const cameraId =
                devices.find(
                    (device) =>
                        device.label &&
                        device.label.toLowerCase().includes('back')
                )?.id || devices[0].id;

            const config = {
                fps: 10,
                qrbox: { width: 250, height: 250 },
                aspectRatio: 4 / 3,
                formatsToSupport: [
                    Html5QrcodeSupportedFormats.QR_CODE,
                    Html5QrcodeSupportedFormats.CODE_39,
                    Html5QrcodeSupportedFormats.CODE_128,
                ],
            };

            const processQRCodeDebounced = debounce(processQRCode, 2500);

            const qrCodeSuccessCallback = (decodedText: string) => {
                const currentTime = Date.now();
                // Kiểm tra thời gian và trạng thái xử lý
                if (
                    !processingRef.current &&
                    currentTime - lastScanTimeRef.current > 2500
                ) {
                    console.log('QR Code detected:', decodedText);
                    setNoQrFound(false);
                    lastScanTimeRef.current = currentTime;
                    processingRef.current = true;
                    processQRCodeDebounced(decodedText);
                }
            };

            await html5QrCodeRef.current.start(
                { deviceId: { exact: cameraId } },
                config,
                qrCodeSuccessCallback,
                () => {
                    // Không log lỗi NotFoundException
                }
            );

            setCameraActive(true);
            setScanning(false);
            setIsTransitioning(false);
        } catch (error) {
            console.error('Error starting camera:', error);
            showAlert(
                error instanceof Error
                    ? error.message
                    : 'Không thể khởi động camera',
                'error'
            );
            setScanning(false);
            setIsTransitioning(false);
        }
    }, [cameraActive, isTransitioning, showAlert, debounce]);

    // Điều khiển bật/tắt máy quét
    useEffect(() => {
        let timeoutId: NodeJS.Timeout;

        const controlScanner = async () => {
            if (isActive && !cameraActive && !isTransitioning) {
                await startScanner();
            } else if (!isActive && cameraActive && !isTransitioning) {
                await stopScanner();
            }
        };

        timeoutId = setTimeout(controlScanner, 100);
        return () => clearTimeout(timeoutId);
    }, [isActive, cameraActive, isTransitioning, startScanner, stopScanner]);

    // Xử lý mã QR
    const processQRCode = async (qrString: string) => {
        console.log('Processing QR Code:', qrString);
        setScanning(true);
        setIsProcessed(true);

        try {
            showAlert('Đang xử lý mã QR...', 'info');

            let numberId = qrString.includes('|')
                ? qrString.split('|')[0]
                : qrString.trim();
            if (!numberId)
                throw new Error('Không tìm thấy numberId trong mã QR.');

            console.log('Sending request with numberId:', numberId);
            const response = await axiosJwt.post(
                `/personnel/check-in-out?numberId=${numberId}`
            );
            const personnel = response.data.data;
            console.log('API response:', personnel);
            if (!personnel) throw new Error('Không tìm thấy nhân viên.');

            const status = personnel.status === 1 ? 'Check In' : 'Check Out';
            showAlert(
                `Đã ${status} thành công cho ${personnel.fullName}`,
                'success'
            );

            dispatch(
                setPersonnelData({
                    id: personnel.id,
                    username: personnel.fullName,
                    numberId: personnel.numberId,
                    classify: personnel.rolePersonnels?.roleName || 'Unknown',
                    status,
                    avatar: personnel.avatarUrl || '',
                    time: formatDateTime(new Date().toString()),
                    phoneNumber: personnel.phoneNumber || '',
                    gender: personnel.gender || '',
                    address: personnel.address || '',
                    dateOfBirth: personnel.dateOfBirth || '',
                })
            );

            // Dừng máy quét sau khi xử lý thành công
            await stopScanner();
        } catch (error: any) {
            console.error('Error processing QR code:', error);
            showAlert(
                error.response?.data?.message ||
                    error.message ||
                    'Lỗi khi xử lý check-in/check-out',
                'error'
            );
            processingRef.current = false;
            setIsProcessed(false);
        } finally {
            setTimeout(() => {
                setScanning(false);
                processingRef.current = false;
            }, 2000);
        }
    };

    const retryScanning = () => {
        processingRef.current = false;
        setScanning(false);
        setIsProcessed(false);
        startScanner();
    };

    // Phần render giữ nguyên như cũ
    return (
        <>
            {AlertComponent}

            <div
                ref={containerRef}
                className="relative w-full max-w-[640px] mx-auto overflow-hidden"
                style={{ aspectRatio: '4/3', maxHeight: '480px' }}
            >
                <div
                    id="qr-scanner"
                    className="w-full h-full"
                    style={{ position: 'relative', overflow: 'hidden' }}
                ></div>

                {cameraActive && (
                    <div
                        className="absolute top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2 
                              w-[250px] h-[250px] border-2 border-white rounded-lg pointer-events-none z-10"
                    >
                        <div className="absolute top-0 left-0 w-5 h-5 border-t-2 border-l-2 border-green-500"></div>
                        <div className="absolute top-0 right-0 w-5 h-5 border-t-2 border-r-2 border-green-500"></div>
                        <div className="absolute bottom-0 left-0 w-5 h-5 border-b-2 border-l-2 border-green-500"></div>
                        <div className="absolute bottom-0 right-0 w-5 h-5 border-b-2 border-r-2 border-green-500"></div>
                    </div>
                )}

                {isActive && !cameraActive && (
                    <div className="absolute inset-0 flex items-center justify-center bg-black bg-opacity-70 z-20">
                        <div className="bg-white p-4 rounded-md">
                            <p className="text-center">
                                Đang khởi tạo camera...
                            </p>
                        </div>
                    </div>
                )}

                {scanning && cameraActive && (
                    <div className="absolute inset-0 flex items-center justify-center bg-black bg-opacity-50 z-20">
                        <div className="bg-white p-4 rounded-md">
                            <p className="text-center mb-2">
                                Đang xử lý mã QR...
                            </p>
                            <button
                                onClick={retryScanning}
                                className="mt-2 px-4 py-2 bg-blue-500 text-white rounded-md w-full"
                            >
                                Thử lại
                            </button>
                        </div>
                    </div>
                )}

                {isActive && cameraActive && !scanning && (
                    <div className="absolute bottom-4 left-1/2 transform -translate-x-1/2 z-10">
                        <p className="text-center text-white bg-black bg-opacity-50 p-2 rounded text-sm">
                            Đặt mã QR vào khung và giữ yên
                        </p>
                    </div>
                )}

                {isActive && cameraActive && !scanning && noQrFound && (
                    <div className="absolute top-4 left-1/2 transform -translate-x-1/2 z-10">
                        <p className="text-center text-white bg-red-500 bg-opacity-75 p-2 rounded text-sm">
                            Không tìm thấy mã QR. Vui lòng đặt mã vào khung.
                        </p>
                    </div>
                )}
            </div>
        </>
    );
};

export default Webcam;
