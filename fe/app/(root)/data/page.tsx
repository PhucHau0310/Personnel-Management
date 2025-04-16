'use client';

import * as React from 'react';
import { DataGrid, GridColDef, GridVisibilityOffIcon } from '@mui/x-data-grid';
import Paper from '@mui/material/Paper';
import axiosJwt from '@/helper/axiosJwt';
import useAlert from '@/hooks/useAlert';
import * as XLSX from 'xlsx';
import { Button } from '@mui/material';

interface Column {
    field: string;
    headerName: string;
    width: number;
    flex: number;
}

const Data = () => {
    const [searchTerm, setSearchTerm] = React.useState('');
    const [selectedDate, setSelectedDate] = React.useState('');
    const [selectedGroup, setSelectedGroup] = React.useState('all');
    const [selectedStatus, setSelectedStatus] = React.useState('all');
    const [historyData, setHistoryData] = React.useState<any[]>([]);
    const [rolePersonnels, setRolePersonnels] = React.useState<any[]>([]);
    const [showExcelPreviewModal, setShowExcelPreviewModal] =
        React.useState(false);
    const [excelPreviewData, setExcelPreviewData] = React.useState<
        {
            STT: any;
            'Họ & Tên': any;
            CCCD: any;
            'Số điện thoại': any;
            'Phân loại': any;
            'Trạng thái': any;
            'Thời gian': string;
            'Ghi chú': any;
        }[]
    >([]);
    const [excelPreviewColumns, setExcelPreviewColumns] = React.useState<
        Column[]
    >([]);

    const { showAlert, AlertComponent } = useAlert();

    // Handlers for filters
    const handleSearchChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setSearchTerm(e.target.value);
    };

    const handleDateChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setSelectedDate(e.target.value);
    };

    const handleGroupChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        setSelectedGroup(e.target.value);
    };

    const handleStatusChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        setSelectedStatus(e.target.value);
    };

    const handleResetFilters = () => {
        setSearchTerm('');
        setSelectedDate('');
        setSelectedGroup('all');
        setSelectedStatus('all');
    };

    // Excel preview and export logic (giữ nguyên)
    const handleExcelPreview = () => {
        try {
            const dataToExport =
                filteredData.length > 0 ? filteredData : historyData;
            const previewData = dataToExport.map((row: any) => ({
                STT: row.stt,
                'Họ & Tên': row.hoVaTen,
                CCCD: row.cccd,
                'Số điện thoại': row.sdt,
                'Phân loại': row.phanLoai,
                'Trạng thái': row.trangThai,
                'Thời gian': row.thoiGian
                    ? new Date(row.thoiGian).toLocaleString()
                    : '',
                'Ghi chú': row.note,
            }));

            setExcelPreviewData(previewData);

            if (previewData.length > 0) {
                const sampleRow = previewData[0];
                const previewColumns = Object.keys(sampleRow).map((key) => ({
                    field: key,
                    headerName: key,
                    width: 150,
                    flex: 1,
                }));
                setExcelPreviewColumns(previewColumns);
            }

            setShowExcelPreviewModal(true);
        } catch (error) {
            console.error('Error generating Excel preview:', error);
            showAlert('Lỗi khi tạo xem trước Excel.', 'error');
        }
    };

    const closeExcelPreviewModal = () => {
        setShowExcelPreviewModal(false);
        setExcelPreviewData([]);
        setExcelPreviewColumns([]);
    };

    const handleExportExcel = () => {
        try {
            showAlert('Đang xuất file Excel...', 'info');
            const dataToExport =
                filteredData.length > 0 ? filteredData : historyData;
            const wb = XLSX.utils.book_new();
            const excelData = dataToExport.map((row: any) => ({
                STT: row.stt,
                'Họ & Tên': row.hoVaTen,
                CCCD: row.cccd,
                'Số điện thoại': row.sdt,
                'Phân loại': row.phanLoai,
                'Trạng thái': row.trangThai,
                'Thời gian': row.thoiGian
                    ? new Date(row.thoiGian).toLocaleString()
                    : '',
                'Ghi chú': row.note,
            }));

            const ws = XLSX.utils.json_to_sheet(excelData);
            XLSX.utils.book_append_sheet(wb, ws, 'Personnel Data');
            XLSX.writeFile(wb, 'personnel_data.xlsx');
            showAlert('Đã xuất file Excel thành công.', 'success');
        } catch (error) {
            console.error('Error exporting to Excel:', error);
            showAlert('Lỗi khi xuất file Excel.', 'error');
        }
    };

    // Fetch data
    const fetchPersonnelHistory = async () => {
        try {
            const response = await axiosJwt.get('/personnel/history');
            if (response.data && response.data.data) {
                setHistoryData(
                    response.data.data.map((item: any, index: number) => ({
                        id: item.id,
                        stt: index + 1,
                        hoVaTen: item.personnel?.fullName || 'N/A',
                        cccd: item.personnel?.numberId || 'N/A',
                        sdt: item.personnel?.phoneNumber || 'N/A',
                        phanLoai:
                            item.personnel?.rolePersonnels?.roleName ||
                            'Unknown',
                        trangThai: item.status === 1 ? 'Check In' : 'Check Out',
                        thoiGian: item.timestamp,
                        note: item.note || '',
                    }))
                );
            }
        } catch (error) {
            console.error('Error fetching history:', error);
            showAlert('Error loading history data', 'error');
        }
    };

    const fetchRolePersonnels = async () => {
        try {
            const res = await axiosJwt.get('/personnel/roles', {
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                },
            });
            let rolesData = res.data.data.$values || res.data.data || [];
            const roleData = rolesData.map((role: any, index: number) => ({
                stt: index + 1,
                id: role.id,
                tenNhom: role.roleName,
                ngayTao: role.createdAt,
                ngayCapNhat: role.updatedAt,
            }));
            setRolePersonnels(roleData);
        } catch (error) {
            console.log('Error fetching roles:', error);
        }
    };

    React.useEffect(() => {
        fetchPersonnelHistory();
        fetchRolePersonnels();
    }, []);

    // Define columns for DataGrid
    const columns: GridColDef[] = [
        { field: 'stt', headerName: 'STT', width: 100 },
        { field: 'hoVaTen', headerName: 'Họ & Tên', width: 180 },
        { field: 'cccd', headerName: 'CCCD', width: 180 },
        { field: 'sdt', headerName: 'Số điện thoại', width: 150 },
        { field: 'phanLoai', headerName: 'Phân loại', width: 150 },
        { field: 'trangThai', headerName: 'Trạng thái', width: 150 },
        {
            field: 'thoiGian',
            headerName: 'Thời gian',
            width: 190,
            valueGetter: (value) => new Date(value),
        },
        { field: 'note', headerName: 'Ghi chú', width: 250 },
    ];

    // Enhanced filtering logic
    const filteredData = historyData.filter((row: any) => {
        const lowerSearchTerm = searchTerm.toLowerCase();

        // Multi-field search
        const matchesSearch =
            searchTerm === '' ||
            row.hoVaTen.toLowerCase().includes(lowerSearchTerm) ||
            row.cccd.toLowerCase().includes(lowerSearchTerm) ||
            (row.sdt && row.sdt.toLowerCase().includes(lowerSearchTerm)) ||
            (row.phanLoai &&
                row.phanLoai.toLowerCase().includes(lowerSearchTerm)) ||
            (row.note && row.note.toLowerCase().includes(lowerSearchTerm));

        // Date filter
        const matchesDate =
            selectedDate === '' ||
            (row.thoiGian &&
                new Date(row.thoiGian).toISOString().slice(0, 10) ===
                    selectedDate);

        // Group filter
        const matchesGroup =
            selectedGroup === 'all' || row.phanLoai === selectedGroup;

        // Status filter
        const matchesStatus =
            selectedStatus === 'all' ||
            row.trangThai ===
                (selectedStatus === 'checkIn' ? 'Check In' : 'Check Out');

        return matchesSearch && matchesDate && matchesGroup && matchesStatus;
    });

    return (
        <>
            {AlertComponent}

            {showExcelPreviewModal && (
                <div className="fixed inset-0 z-50 flex items-center justify-center overflow-auto">
                    <div
                        className="absolute inset-0 bg-black bg-opacity-60"
                        onClick={closeExcelPreviewModal}
                    ></div>
                    <div className="bg-white z-50 shadow-md shadow-black p-6 rounded-md relative w-11/12 h-5/6 max-w-6xl overflow-auto">
                        <h2 className="font-semibold text-xl mb-4 text-blue-900">
                            Xem trước file Excel
                        </h2>
                        <div className="mb-4">
                            <p className="text-gray-600">
                                Dưới đây là xem trước dữ liệu sẽ được xuất ra
                                file Excel.
                            </p>
                        </div>
                        <div className="h-4/6 mb-4">
                            {excelPreviewData.length > 0 ? (
                                <Paper sx={{ width: '100%', height: '100%' }}>
                                    <DataGrid
                                        rows={excelPreviewData.map(
                                            (row: any, index) => ({
                                                ...row,
                                                id: index,
                                            })
                                        )}
                                        columns={excelPreviewColumns}
                                        initialState={{
                                            pagination: {
                                                paginationModel: {
                                                    page: 0,
                                                    pageSize: 10,
                                                },
                                            },
                                        }}
                                        pageSizeOptions={[10, 25, 50]}
                                        disableRowSelectionOnClick
                                        sx={{ border: 0 }}
                                    />
                                </Paper>
                            ) : (
                                <div className="flex justify-center items-center h-full">
                                    <div className="flex flex-col items-center">
                                        <GridVisibilityOffIcon
                                            sx={{ fontSize: 40, color: 'gray' }}
                                        />
                                        <span className="mt-2 text-gray-500">
                                            Không có dữ liệu
                                        </span>
                                    </div>
                                </div>
                            )}
                        </div>
                        <div className="flex justify-end space-x-4">
                            <Button
                                variant="outlined"
                                onClick={closeExcelPreviewModal}
                            >
                                Đóng
                            </Button>
                            <Button
                                variant="contained"
                                color="success"
                                onClick={handleExportExcel}
                                disabled={excelPreviewData.length === 0}
                            >
                                Xuất Excel
                            </Button>
                        </div>
                    </div>
                </div>
            )}

            <div>
                <div className="p-4 shadow-md rounded-md flex flex-row justify-between items-center mb-6 gap-4">
                    <div>
                        <label htmlFor="search" className="mr-2">
                            Tìm kiếm
                        </label>
                        <input
                            id="search"
                            type="text"
                            className="border border-gray-300 rounded-md p-1"
                            value={searchTerm}
                            onChange={handleSearchChange}
                            placeholder="Tìm theo tên, CCCD, SĐT, phân loại, ghi chú..."
                        />
                    </div>

                    <div>
                        <label htmlFor="date" className="mr-2">
                            Ngày
                        </label>
                        <input
                            id="date"
                            type="date"
                            className="border border-gray-300 rounded-md p-1"
                            value={selectedDate}
                            onChange={handleDateChange}
                        />
                    </div>

                    <div>
                        <label htmlFor="nhom" className="mr-2">
                            Nhóm
                        </label>
                        <select
                            name="nhom"
                            id="nhom"
                            className="border-2 border-gray-300 rounded-md py-1"
                            value={selectedGroup}
                            onChange={handleGroupChange}
                        >
                            <option value="all">Tất cả</option>
                            {rolePersonnels.map((role: any) => (
                                <option key={role.id} value={role.tenNhom}>
                                    {role.tenNhom}
                                </option>
                            ))}
                        </select>
                    </div>

                    <div>
                        <label htmlFor="status" className="mr-2">
                            Trạng thái
                        </label>
                        <select
                            name="status"
                            id="status"
                            className="border-2 border-gray-300 rounded-md py-1"
                            value={selectedStatus}
                            onChange={handleStatusChange}
                        >
                            <option value="all">Tất cả</option>
                            <option value="checkIn">Check In</option>
                            <option value="checkOut">Check Out</option>
                        </select>
                    </div>

                    <div className="flex flex-row items-center gap-3">
                        <button
                            className="bg-gray-500 text-white p-2 rounded-md hover:opacity-80"
                            onClick={handleResetFilters}
                        >
                            Reset
                        </button>
                        <button
                            className="bg-blue-500 text-white p-2 rounded-md hover:opacity-80"
                            onClick={handleExcelPreview}
                        >
                            Xem trước
                        </button>
                        <button
                            className="bg-green-500 text-white p-2 rounded-md hover:opacity-80"
                            onClick={handleExportExcel}
                        >
                            Xuất Excel
                        </button>
                    </div>
                </div>

                <Paper sx={{ width: '100%' }}>
                    <DataGrid
                        rows={filteredData}
                        columns={columns}
                        getRowId={(row) => row.id}
                        initialState={{
                            pagination: {
                                paginationModel: { page: 0, pageSize: 5 },
                            },
                        }}
                        pageSizeOptions={[5, 10, 25]}
                        checkboxSelection
                        sx={{ border: 0 }}
                        slots={{
                            noRowsOverlay: () => (
                                <div className="flex justify-center items-center h-full">
                                    <div className="flex flex-col items-center">
                                        <GridVisibilityOffIcon
                                            sx={{ fontSize: 40, color: 'gray' }}
                                        />
                                        <span className="mt-2 text-gray-500">
                                            Không có dữ liệu
                                        </span>
                                    </div>
                                </div>
                            ),
                        }}
                    />
                </Paper>
            </div>
        </>
    );
};

export default Data;
