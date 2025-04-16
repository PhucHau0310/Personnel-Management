'use client';

import avatarDefault from '../../public/avatar-default.png';
import * as React from 'react';
import { DataGrid, GridColDef } from '@mui/x-data-grid';
import Paper from '@mui/material/Paper';
import Webcam from '@/components/Webcam';
import { useState, useEffect } from 'react';
import { useAppDispatch, useAppSelector } from '@/hooks/reduxHooks';
import { resetPersonnelData, setPersonnelData } from '@/store/personnelSlice';
import axiosJwt from '@/helper/axiosJwt';
import useAlert from '@/hooks/useAlert';
import { formatDateTime } from '@/utils/formatDateTime';

const columns: GridColDef[] = [
    { field: 'stt', headerName: 'STT', width: 70 },
    { field: 'hoVaTen', headerName: 'Họ & Tên', width: 180 },
    { field: 'phanLoai', headerName: 'Phân loại', width: 120 },
    { field: 'trangThai', headerName: 'Trạng thái', width: 120 },
    {
        field: 'thoiGian',
        headerName: 'Thời gian',
        type: 'dateTime',
        width: 200,
        valueGetter: (value, row) => new Date(row.thoiGian),
    },
];

const paginationModel = { page: 0, pageSize: 5 };

export default function Home() {
    const [isWebcamActive, setWebcamActive] = useState(false);
    const [isLoading, setLoading] = useState(false);
    const [historyData, setHistoryData] = useState<any[]>([]);
    const [image, setImage] = useState<File | null>(null);
    const [source, setSource] = useState<'picture' | 'camera'>('picture'); // Quản lý nguồn vào

    const dispatch = useAppDispatch();
    const personnelData = useAppSelector((state) => state.personnel);
    const { showAlert, AlertComponent } = useAlert();

    const convertStatus: Record<number, string> = {
        1: 'Check In',
        2: 'Check Out',
    };

    const handleOpenCamera = () => {
        setWebcamActive(true); // Bật webcam
    };

    const handleCloseCamera = () => {
        setWebcamActive(false); // Tắt webcam
        // Không reload trang nữa để giữ trạng thái
        window.location.reload();
    };

    const handleSubmit = async () => {
        if (source === 'picture') {
            // Xử lý khi chọn ảnh
            if (!image) {
                showAlert('Vui lòng chọn ảnh', 'info');
                return;
            }

            setLoading(true);
            try {
                const formData = new FormData();
                formData.append('cccd', image);

                const res = await fetch(
                    'https://localhost:3031/api/personnel/check-authen-image',
                    {
                        method: 'POST',
                        body: formData,
                    }
                );

                const result = await res.json();

                if (res.status === 401) {
                    showAlert(
                        'Bạn không phải nhân viên của chúng tôi',
                        'warning'
                    );
                    return;
                }

                showAlert(result.message, 'info');

                const personnelData = {
                    id: result.data.id,
                    username: result.data.fullName,
                    numberId: result.data.numberId,
                    classify: result.data.rolePersonnels?.roleName || 'Unknown',
                    status:
                        convertStatus[
                            result.data.status as keyof typeof convertStatus
                        ] || 'Không xác định',
                    avatar: result.data.avatarUrl,
                    time: formatDateTime(new Date().toString()),
                    phoneNumber: result.data.phoneNumber,
                    gender: result.data.gender,
                    address: result.data.address,
                    dateOfBirth: result.data.dateOfBirth,
                };

                dispatch(setPersonnelData(personnelData));
                fetchPersonnelHistory();
            } catch (error) {
                console.log(error);
                showAlert('Lỗi trong quá trình xác thực', 'error');
            } finally {
                setLoading(false);
                setImage(null);
            }
        } else if (source === 'camera') {
            // Xử lý khi chọn camera
            handleOpenCamera();
        }
    };

    const handleImageChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const file = e.target.files?.[0];
        if (file) {
            setImage(file);
        }
    };

    const handleResetPersonnel = () => {
        dispatch(resetPersonnelData());
        showAlert('Reset thông tin nhân viên thành công', 'success');
    };

    const fetchPersonnelHistory = async () => {
        setLoading(true);
        try {
            const response = await axiosJwt.get('/personnel/history');
            if (response.data && response.data.data) {
                setHistoryData(
                    response.data.data.map((item: any, index: number) => ({
                        id: item.id,
                        stt: index + 1,
                        hoVaTen: item.personnel.fullName,
                        phanLoai:
                            item.personnel.rolePersonnels?.roleName ||
                            'Unknown',
                        trangThai: item.status === 1 ? 'Check In' : 'Check Out',
                        thoiGian: item.timestamp,
                    }))
                );
            }
        } catch (error) {
            console.error('Error fetching history:', error);
            showAlert('Error loading history data', 'error');
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchPersonnelHistory();
        const interval = setInterval(() => fetchPersonnelHistory(), 30000);
        return () => clearInterval(interval);
    }, []);

    return (
        <>
            {AlertComponent}
            <div className="w-full flex flex-row gap-4">
                <div className="w-1/3">
                    <div className="flex flex-col items-center py-4 shadow-xl rounded-md">
                        <h1 className="text-2xl font-semibold mb-8">
                            Thông tin nhân viên
                        </h1>
                        <div className="flex flex-row items-start gap-5 px-4">
                            <img
                                src={personnelData.avatar || avatarDefault.src}
                                alt="avatar"
                                className="w-36 h-36 p-2 border-2 border-gray-300"
                            />
                            <div>
                                <div className="mb-2">
                                    <p className="text-xl font-semibold">
                                        Họ và tên:{' '}
                                    </p>
                                    <p className="text-base">
                                        {personnelData.username}
                                    </p>
                                </div>
                                <div className="mb-2">
                                    <p className="text-xl font-semibold">
                                        ID nhân viên:{' '}
                                    </p>
                                    <p className="text-base">
                                        {personnelData.id}
                                    </p>
                                </div>
                                <div className="mb-2">
                                    <p className="text-xl font-semibold">
                                        Phân loại:{' '}
                                    </p>
                                    <p className="text-base">
                                        {personnelData.classify}
                                    </p>
                                </div>
                                <div className="mb-2">
                                    <p className="text-xl font-semibold">
                                        Trạng thái:{' '}
                                    </p>
                                    <p className="text-base">
                                        {personnelData.status}
                                    </p>
                                </div>
                                <div className="mb-2">
                                    <p className="text-xl font-semibold">
                                        Thời gian:{' '}
                                    </p>
                                    <p className="text-base">
                                        {personnelData.time}
                                    </p>
                                </div>
                            </div>
                        </div>
                        {personnelData?.numberId?.length > 0 && (
                            <button
                                onClick={handleResetPersonnel}
                                className="bg-green-700 shadow-md px-6 py-2 text-white rounded-md mt-6 hover:opacity-85"
                            >
                                Reset
                            </button>
                        )}
                    </div>

                    <div className="mt-4 shadow-xl rounded-md p-4">
                        <h2 className="text-center text-2xl font-semibold mb-8">
                            Trung tâm xử lý QR
                        </h2>
                        <div className="flex flex-row items-center justify-between">
                            <span className="text-lg">Nguồn vào: </span>
                            <select
                                name="nguonvao"
                                id="nguonvao"
                                className="w-[30%] border-2 border-gray-300 rounded-md py-1 px-2"
                                value={source}
                                onChange={(e) =>
                                    setSource(
                                        e.target.value as 'picture' | 'camera'
                                    )
                                }
                            >
                                <option value="picture">Ảnh</option>
                                <option value="camera">Iriun Webcam</option>
                            </select>
                            <button
                                onClick={handleSubmit}
                                className="bg-green-700 text-white py-1.5 px-7 rounded-md hover:opacity-80"
                                disabled={isLoading}
                            >
                                {isLoading ? 'Loading...' : 'Submit'}
                            </button>
                        </div>

                        <div className="h-56 bg-black my-12 flex justify-center items-center">
                            {source === 'camera' && !isWebcamActive && (
                                <p className="text-white text-center px-4">
                                    Hãy nhấn Submit để bật camera và hãy chắc
                                    chắn là bạn đã có Iriun Webcam trong máy
                                </p>
                            )}
                            {/* {source === 'camera' && isWebcamActive && (
                                <Webcam isActive={isWebcamActive} />
                            )} */}
                            {source === 'picture' && (
                                <div className="flex flex-col items-center gap-3">
                                    <label className="cursor-pointer bg-blue-500 text-white px-4 py-2 rounded-lg hover:bg-blue-600">
                                        Chọn ảnh CCCD
                                        <input
                                            type="file"
                                            accept="image/*"
                                            className="hidden"
                                            onChange={handleImageChange}
                                        />
                                    </label>
                                    {image && (
                                        <img
                                            src={URL.createObjectURL(image)}
                                            alt="Preview"
                                            className="w-60 h-40 rounded-lg shadow-md"
                                        />
                                    )}
                                </div>
                            )}
                        </div>
                    </div>
                </div>

                <div className="w-2/3 flex flex-col">
                    <h2 className="mb-3 px-1 rounded-sm shadow-md text-center font-semibold text-lg bg-green-500 text-white w-[28%] ml-auto">
                        Lịch sử ra vào của nhân viên
                    </h2>
                    <Paper sx={{ width: '100%', height: '100%' }}>
                        <DataGrid
                            rows={historyData}
                            columns={columns}
                            getRowId={(row) => row.id}
                            initialState={{ pagination: { paginationModel } }}
                            pageSizeOptions={[5, 10, 20]}
                            checkboxSelection
                            sx={{ border: 0 }}
                        />
                    </Paper>
                </div>
            </div>

            {/* Hiển thị Webcam ở giữa màn hình */}
            {isWebcamActive && (
                <div className="fixed inset-0 flex items-center justify-center bg-black bg-opacity-70 z-[9999]">
                    <div className="relative bg-white p-4 rounded-md w-full max-w-[680px]">
                        <div className="mb-2 flex justify-between items-center">
                            <h3 className="text-lg font-medium">Quét mã QR</h3>
                            <button
                                onClick={handleCloseCamera}
                                className="bg-red-500 text-white px-3 py-1 rounded-md hover:bg-red-600"
                            >
                                Đóng
                            </button>
                        </div>
                        <Webcam isActive={isWebcamActive} />
                        <div className="mt-3 text-center text-sm text-gray-600">
                            <p>Đặt mã QR vào khung và giữ yên để quét</p>
                        </div>
                    </div>
                </div>
            )}
        </>
    );
}
