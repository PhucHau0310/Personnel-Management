'use client';

import * as React from 'react';
import Tabs from '@mui/material/Tabs';
import Tab from '@mui/material/Tab';
import Box from '@mui/material/Box';
import {
    DataGrid,
    GridAddIcon,
    GridColDef,
    GridVisibilityOffIcon,
} from '@mui/x-data-grid';
import Paper from '@mui/material/Paper';
import {
    Button,
    FormControl,
    InputLabel,
    MenuItem,
    Select,
    SelectChangeEvent,
    TextField,
} from '@mui/material';
import axiosJwt from '@/helper/axiosJwt';
import useAlert from '@/hooks/useAlert';

interface TabPanelProps {
    children?: React.ReactNode;
    index: number;
    value: number;
}

interface PersonnelFormData {
    id: string;
    hoVaTen: string;
    cccd: string;
    sdt: string;
    idNguoiDung: string;
    gender: string;
    dateOfBirth: string;
    address: string;
    phanLoai: string;
    trangThai: string | number;
}

interface NewPersonnelFormData {
    rolePersonnelId: string;
    phoneNumber: string;
}

interface RolePersonnelFormData {
    id: string;
    roleName: string;
}

function CustomTabPanel(props: TabPanelProps) {
    const { children, value, index, ...other } = props;

    return (
        <div
            role="tabpanel"
            hidden={value !== index}
            id={`simple-tabpanel-${index}`}
            aria-labelledby={`simple-tab-${index}`}
            {...other}
        >
            {value === index && <Box sx={{ p: 3 }}>{children}</Box>}
        </div>
    );
}

function a11yProps(index: number) {
    return {
        id: `simple-tab-${index}`,
        'aria-controls': `simple-tabpanel-${index}`,
    };
}

const paginationModel = { page: 0, pageSize: 5 };

const List = () => {
    const [value, setValue] = React.useState(0);
    const [image, setImage] = React.useState<File | null>(null);
    const [isLoading, setLoading] = React.useState(false);
    const [formattedPersonnels, setFormattedPersonnels] = React.useState<any>(
        []
    );
    const [formattedGroups, setFormattedGroups] = React.useState<any>([]);

    // Delete modal state
    const [showDeleteModal, setShowDeleteModal] = React.useState(false);
    const [personnelToDelete, setPersonnelToDelete] = React.useState<
        string | null
    >(null);
    const [groupToDelete, setGroupToDelete] = React.useState<string | null>(
        null
    );

    // Edit personnel modal state
    const [showEditModal, setShowEditModal] = React.useState(false);
    const [personnelFormData, setPersonnelFormData] =
        React.useState<PersonnelFormData>({
            id: '',
            hoVaTen: '',
            cccd: '',
            sdt: '',
            idNguoiDung: '',
            gender: '',
            dateOfBirth: '',
            address: '',
            phanLoai: '',
            trangThai: 1,
        });

    // Add personnel modal state
    const [showAddModal, setShowAddModal] = React.useState(false);
    const [newPersonnelFormData, setNewPersonnelFormData] =
        React.useState<NewPersonnelFormData>({
            rolePersonnelId: '',
            phoneNumber: '',
        });

    // Add/Edit role modal state
    const [showRoleModal, setShowRoleModal] = React.useState(false);
    const [isEditRole, setIsEditRole] = React.useState(false);
    const [roleFormData, setRoleFormData] =
        React.useState<RolePersonnelFormData>({
            id: '',
            roleName: '',
        });

    const { showAlert, AlertComponent } = useAlert();

    const convertStatus: Record<number, string> = {
        1: 'Check In',
        2: 'Check Out',
    };

    const handleChange = (event: React.SyntheticEvent, newValue: number) => {
        setValue(newValue);
    };

    // Edit handlers
    const handleEdit = (row: any) => {
        if (value === 0) {
            // Edit Personnel
            const roleGroup = formattedGroups.find(
                (role: any) => role.tenNhom === row.phanLoai
            );
            const roleId = roleGroup ? roleGroup.id : '';
            const trangThaiKey =
                Object.keys(convertStatus).find(
                    (key) => convertStatus[Number(key)] === row.trangThai
                ) || '1';

            setPersonnelFormData({
                id: row.id || row.idNguoiDung,
                hoVaTen: row.hoVaTen || '',
                cccd: row.cccd || '',
                sdt: row.sdt || '',
                gender: row.gender || '',
                dateOfBirth: row.dateOfBirth || '',
                address: row.address || '',
                idNguoiDung: row.idNguoiDung || '',
                phanLoai: roleId,
                trangThai: trangThaiKey,
            });
            setShowEditModal(true);
        } else {
            // Edit Role
            setRoleFormData({
                id: row.id,
                roleName: row.tenNhom,
            });
            setIsEditRole(true);
            setShowRoleModal(true);
        }
    };

    // Delete handlers
    const handleDelete = (id: string) => {
        if (value === 0) {
            setPersonnelToDelete(id);
        } else {
            setGroupToDelete(id);
        }
        setShowDeleteModal(true);
    };

    const confirmDelete = async () => {
        if (value === 0 && personnelToDelete) {
            // Delete Personnel
            try {
                const response = await axiosJwt.delete(
                    `/personnel/${personnelToDelete}`
                );
                if (response.status === 200) {
                    setFormattedPersonnels(
                        formattedPersonnels.filter(
                            (p: any) => p.id !== personnelToDelete
                        )
                    );
                    showAlert('Nhân viên đã được xóa thành công.', 'success');
                } else {
                    showAlert('Lỗi khi xóa nhân viên.', 'error');
                }
            } catch (error) {
                console.log('Lỗi trong quá trình xóa nhân viên.', error);
                showAlert('Lỗi trong quá trình xóa nhân viên.', 'error');
            }
        } else if (value === 1 && groupToDelete) {
            // Delete Role
            try {
                const response = await axiosJwt.delete(
                    `/personnel/roles/${groupToDelete}`
                );
                if (response.status === 200) {
                    setFormattedGroups(
                        formattedGroups.filter(
                            (g: any) => g.id !== groupToDelete
                        )
                    );
                    showAlert('Nhóm đã được xóa thành công.', 'success');
                } else {
                    showAlert('Lỗi khi xóa nhóm.', 'error');
                }
            } catch (error) {
                console.log('Lỗi trong quá trình xóa nhóm.', error);
                showAlert('Lỗi trong quá trình xóa nhóm.', 'error');
            }
        }
        setShowDeleteModal(false);
        setPersonnelToDelete(null);
        setGroupToDelete(null);
    };

    // Input change handlers
    const handleInputChange = (
        event:
            | React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>
            | SelectChangeEvent<unknown>
    ) => {
        const { name, value } = event.target;
        setPersonnelFormData((prev) => ({ ...prev, [name]: value }));
    };

    const handleNewPersonnelInputChange = (
        event:
            | React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>
            | SelectChangeEvent<unknown>
    ) => {
        const { name, value } = event.target;
        setNewPersonnelFormData((prev) => ({ ...prev, [name]: value }));
    };

    const handleRoleInputChange = (
        event: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>
    ) => {
        const { name, value } = event.target;
        setRoleFormData((prev) => ({ ...prev, [name]: value }));
    };

    // Form submission handlers
    const handleSubmitEdit = async (e: React.FormEvent) => {
        e.preventDefault();
        try {
            const response = await axiosJwt.put(
                `/personnel/${personnelFormData.id}`,
                {
                    fullName: personnelFormData.hoVaTen,
                    numberId: personnelFormData.cccd,
                    phoneNumber: personnelFormData.sdt,
                    rolePersonnelId: personnelFormData.phanLoai,
                    address: personnelFormData.address,
                    dateOfBirth: personnelFormData.dateOfBirth,
                    gender: personnelFormData.gender,
                    status: Number(personnelFormData.trangThai),
                    dateCreatedCccd: null,
                }
            );
            if (response.status === 200) {
                fetchPersonnels(formattedGroups);
                setShowEditModal(false);
                showAlert(
                    'Thông tin nhân viên đã được cập nhật thành công.',
                    'success'
                );
            } else {
                showAlert('Lỗi khi cập nhật thông tin nhân viên.', 'error');
            }
        } catch (error) {
            console.log('Lỗi trong quá trình cập nhật nhân viên.', error);
            showAlert('Lỗi trong quá trình cập nhật nhân viên.', 'error');
        }
    };

    const handleUpload = async () => {
        if (
            !image ||
            !newPersonnelFormData.rolePersonnelId ||
            !newPersonnelFormData.phoneNumber
        ) {
            showAlert('Vui lòng điền đầy đủ thông tin.', 'warning');
            return;
        }
        setLoading(true);
        try {
            const formData = new FormData();
            formData.append('cccd', image);
            const res = await axiosJwt.post(
                `/personnel?rolePersonnelId=${newPersonnelFormData.rolePersonnelId}&phoneNumber=${newPersonnelFormData.phoneNumber}`,
                formData
            );
            if (res.status === 200) {
                showAlert('Thêm nhân viên thành công.', 'success');
                fetchPersonnels(formattedGroups);
                closeAddModal();
            } else {
                showAlert('Không thể thêm nhân viên.', 'error');
            }
        } catch (error) {
            console.log('Lỗi trong quá trình thêm nhân viên.', error);
            showAlert('Lỗi trong quá trình thêm nhân viên.', 'error');
        } finally {
            setLoading(false);
            setImage(null);
        }
    };

    const handleImageChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const file = e.target.files?.[0];
        if (file) setImage(file);
    };

    const handleRoleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        try {
            if (isEditRole) {
                // Edit Role
                const response = await axiosJwt.put(
                    `/personnel/roles/${roleFormData.id}?roleName=${roleFormData.roleName}`
                );
                if (response.status === 200) {
                    await fetchRolePersonnels();
                    showAlert('Nhóm đã được cập nhật thành công.', 'success');
                } else {
                    showAlert('Lỗi khi cập nhật nhóm.', 'error');
                }
            } else {
                // Add Role
                const response = await axiosJwt.post(
                    `/personnel/roles?roleName=${roleFormData.roleName}`
                );
                if (response.status === 200) {
                    await fetchRolePersonnels();
                    showAlert('Nhóm đã được tạo thành công.', 'success');
                } else {
                    showAlert('Lỗi khi tạo nhóm.', 'error');
                }
            }
            setShowRoleModal(false);
            setRoleFormData({ id: '', roleName: '' });
            setIsEditRole(false);
        } catch (error) {
            console.error('Lỗi khi xử lý nhóm:', error);
            showAlert('Lỗi trong quá trình xử lý nhóm.', 'error');
        }
    };

    // Modal control functions
    const closeDeleteModal = () => {
        setShowDeleteModal(false);
        setPersonnelToDelete(null);
        setGroupToDelete(null);
    };

    const closeEditModal = () => {
        setShowEditModal(false);
    };

    const openAddModal = () => {
        setShowAddModal(true);
    };

    const closeAddModal = () => {
        setShowAddModal(false);
        setImage(null);
    };

    const fetchPersonnels = async (roles: any) => {
        try {
            const res = await axiosJwt.get('/personnel', {
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                },
            });
            let personnelsData = res.data.data.$values || res.data.data || [];
            const formattedData = personnelsData.map(
                (personnel: any, index: number) => ({
                    stt: index + 1,
                    id: personnel.id,
                    hoVaTen: personnel.fullName,
                    cccd: personnel.numberId,
                    sdt: personnel.phoneNumber,
                    idNguoiDung: personnel.id,
                    gender: personnel.gender,
                    dateOfBirth: personnel.dateOfBirth,
                    address: personnel.address,
                    phanLoai: roles.find(
                        (role: any) => role.id === personnel.rolePersonnel
                    )?.tenNhom,
                    trangThai:
                        convertStatus[personnel.status] || 'Không xác định',
                    ngayTao: personnel.createdAt,
                    ngayCapNhat: personnel.updatedAt,
                })
            );
            setFormattedPersonnels(formattedData);
        } catch (error) {
            console.log('Failed to get personnels:', error);
            showAlert('Lỗi trong quá trình lấy tất cả nhân viên.', 'error');
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
            setFormattedGroups(roleData);
            return roleData;
        } catch (error) {
            console.log('Error formatting groups:', error);
            return [];
        }
    };

    React.useEffect(() => {
        const fetchData = async () => {
            const roles = await fetchRolePersonnels();
            await fetchPersonnels(roles);
        };
        fetchData();
    }, []);

    const columns: GridColDef[] = [
        { field: 'stt', headerName: 'STT', width: 70 },
        { field: 'hoVaTen', headerName: 'Họ & Tên', width: 160 },
        { field: 'cccd', headerName: 'CCCD', width: 130 },
        { field: 'sdt', headerName: 'Số điện thoại', width: 130 },
        { field: 'idNguoiDung', headerName: 'ID Người dùng', width: 130 },
        { field: 'gender', headerName: 'Giới tính', width: 130 },
        { field: 'dateOfBirth', headerName: 'Ngày sinh', width: 130 },
        { field: 'address', headerName: 'Địa chỉ', width: 130 },
        { field: 'phanLoai', headerName: 'Phân loại', width: 90 },
        // { field: 'trangThai', headerName: 'Trạng thái', width: 120 },
        {
            field: 'ngayTao',
            headerName: 'Ngày tạo',
            type: 'dateTime',
            width: 130,
            valueGetter: (value, row) => new Date(row.ngayTao),
        },
        {
            field: 'ngayCapNhat',
            headerName: 'Ngày cập nhật',
            type: 'dateTime',
            width: 130,
            valueGetter: (value, row) =>
                row.ngayCapNhat ? new Date(row.ngayCapNhat) : null,
        },
        {
            field: 'actions',
            headerName: 'Hành động',
            width: 200,
            renderCell: (params) => (
                <>
                    <Button
                        variant="contained"
                        color="primary"
                        size="small"
                        onClick={() => handleEdit(params.row)}
                        style={{ marginRight: 8 }}
                    >
                        Chỉnh sửa
                    </Button>
                    <Button
                        variant="contained"
                        color="secondary"
                        size="small"
                        onClick={() => handleDelete(params.row.id)}
                    >
                        Xóa
                    </Button>
                </>
            ),
        },
    ];

    const columnsPhanNhom: GridColDef[] = [
        { field: 'stt', headerName: 'STT', width: 100 },
        { field: 'tenNhom', headerName: 'Tên nhóm', width: 200 },
        {
            field: 'ngayTao',
            headerName: 'Ngày tạo',
            type: 'dateTime',
            width: 150,
            valueGetter: (value, row) => new Date(row.ngayTao),
        },
        {
            field: 'ngayCapNhat',
            headerName: 'Ngày cập nhật',
            type: 'dateTime',
            width: 150,
            valueGetter: (value, row) =>
                row.ngayCapNhat ? new Date(row.ngayCapNhat) : null,
        },
        {
            field: 'actions',
            headerName: 'Hành động',
            width: 200,
            renderCell: (params) => (
                <>
                    <Button
                        variant="contained"
                        color="primary"
                        size="small"
                        onClick={() => handleEdit(params.row)}
                        style={{ marginRight: 8 }}
                    >
                        Chỉnh sửa
                    </Button>
                    <Button
                        variant="contained"
                        color="secondary"
                        size="small"
                        onClick={() => handleDelete(params.row.id)}
                    >
                        Xóa
                    </Button>
                </>
            ),
        },
    ];

    return (
        <>
            {AlertComponent}

            {/* Delete Confirmation Modal */}
            {showDeleteModal && (
                <div className="fixed inset-0 z-50 flex items-center justify-center">
                    <div
                        className="absolute inset-0 bg-black bg-opacity-60"
                        onClick={closeDeleteModal}
                    ></div>
                    <div className="bg-white z-50 shadow-md shadow-black px-10 py-6 rounded-md relative">
                        <h2 className="font-semibold text-xl mb-2 text-blue-900">
                            Bạn có chắc chắn muốn xóa{' '}
                            {value === 0 ? 'người dùng' : 'nhóm'} này?
                        </h2>
                        <p className="text-center mb-6">
                            ID:{' '}
                            {value === 0 ? personnelToDelete : groupToDelete}
                        </p>
                        <div className="flex flex-row items-center gap-4 text-white">
                            <button
                                className="w-1/2 bg-gray-500 py-2 rounded-md hover:bg-gray-600 transition-colors"
                                onClick={closeDeleteModal}
                            >
                                Hủy
                            </button>
                            <button
                                className="w-1/2 bg-blue-600 py-2 rounded-md hover:bg-blue-700 transition-colors"
                                onClick={confirmDelete}
                            >
                                Xác nhận
                            </button>
                        </div>
                    </div>
                </div>
            )}

            {/* Edit Personnel Modal */}
            {showEditModal && (
                <div className="fixed inset-0 z-[50] flex items-center justify-center overflow-y-auto">
                    <div
                        className="absolute inset-0 bg-black bg-opacity-60"
                        onClick={closeEditModal}
                    ></div>
                    <div className="bg-white z-50 shadow-md shadow-black px-5 py-4 rounded-md relative w-full max-w-2xl">
                        <h2 className="font-semibold text-lg mb-2 text-blue-900">
                            Chỉnh sửa thông tin
                        </h2>
                        <form onSubmit={handleSubmitEdit}>
                            <div className="grid grid-cols-2 gap-x-4 gap-y-2">
                                <TextField
                                    label="Họ và tên"
                                    name="hoVaTen"
                                    value={personnelFormData.hoVaTen}
                                    onChange={handleInputChange}
                                    fullWidth
                                    required
                                    size="small"
                                    margin="dense"
                                />
                                <TextField
                                    label="CCCD"
                                    name="cccd"
                                    value={personnelFormData.cccd}
                                    onChange={handleInputChange}
                                    fullWidth
                                    required
                                    size="small"
                                    margin="dense"
                                />
                                <TextField
                                    label="Số điện thoại"
                                    name="sdt"
                                    value={personnelFormData.sdt}
                                    onChange={handleInputChange}
                                    fullWidth
                                    required
                                    size="small"
                                    margin="dense"
                                />
                                <TextField
                                    label="Giới tính"
                                    name="gender"
                                    value={personnelFormData.gender}
                                    onChange={handleInputChange}
                                    fullWidth
                                    required
                                    size="small"
                                    margin="dense"
                                />
                                <TextField
                                    label="Ngày sinh"
                                    name="dateOfBirth"
                                    value={personnelFormData.dateOfBirth}
                                    onChange={handleInputChange}
                                    fullWidth
                                    required
                                    size="small"
                                    margin="dense"
                                />
                                <TextField
                                    label="Địa chỉ"
                                    name="address"
                                    value={personnelFormData.address}
                                    onChange={handleInputChange}
                                    fullWidth
                                    required
                                    size="small"
                                    margin="dense"
                                />
                                <FormControl
                                    fullWidth
                                    margin="dense"
                                    size="small"
                                >
                                    <InputLabel id="edit-role-select-label">
                                        Nhóm phân quyền
                                    </InputLabel>
                                    <Select
                                        labelId="edit-role-select-label"
                                        name="phanLoai"
                                        value={personnelFormData.phanLoai}
                                        label="Nhóm phân quyền"
                                        onChange={handleInputChange}
                                    >
                                        {formattedGroups.map((role: any) => (
                                            <MenuItem
                                                key={role.id}
                                                value={role.id}
                                            >
                                                {role.tenNhom}
                                            </MenuItem>
                                        ))}
                                    </Select>
                                </FormControl>
                                <FormControl
                                    fullWidth
                                    margin="dense"
                                    size="small"
                                >
                                    <InputLabel id="trangThai-label">
                                        Trạng thái
                                    </InputLabel>
                                    <Select
                                        labelId="trangThai-label"
                                        name="trangThai"
                                        value={personnelFormData.trangThai}
                                        onChange={handleInputChange}
                                        label="Trạng thái"
                                    >
                                        <MenuItem value="1">Check In</MenuItem>
                                        <MenuItem value="2">Check Out</MenuItem>
                                    </Select>
                                </FormControl>
                            </div>
                            <div className="flex justify-end space-x-3 mt-3">
                                <Button
                                    variant="outlined"
                                    onClick={closeEditModal}
                                    size="small"
                                >
                                    Hủy
                                </Button>
                                <Button
                                    variant="contained"
                                    color="primary"
                                    type="submit"
                                    size="small"
                                >
                                    Lưu thay đổi
                                </Button>
                            </div>
                        </form>
                    </div>
                </div>
            )}

            {/* Add Personnel Modal */}
            {showAddModal && (
                <div className="fixed inset-0 z-50 flex items-center justify-center">
                    <div
                        className="absolute inset-0 bg-black bg-opacity-60"
                        onClick={closeAddModal}
                    ></div>
                    <div className="bg-white z-50 shadow-md shadow-black px-5 py-4 rounded-md relative w-full max-w-sm">
                        <h2 className="font-semibold text-lg mb-2 text-blue-900">
                            Thêm mới nhân viên
                        </h2>
                        <div className="mb-2">
                            <TextField
                                label="Số điện thoại"
                                variant="outlined"
                                fullWidth
                                name="phoneNumber"
                                value={newPersonnelFormData.phoneNumber}
                                onChange={handleNewPersonnelInputChange}
                                margin="dense"
                                required
                                size="small"
                            />
                        </div>
                        <div className="mb-3">
                            <FormControl fullWidth margin="dense" size="small">
                                <InputLabel id="new-role-select-label">
                                    Nhóm phân quyền
                                </InputLabel>
                                <Select
                                    labelId="new-role-select-label"
                                    name="rolePersonnelId"
                                    value={newPersonnelFormData.rolePersonnelId}
                                    label="Nhóm phân quyền"
                                    onChange={handleNewPersonnelInputChange}
                                >
                                    {formattedGroups.map((role: any) => (
                                        <MenuItem key={role.id} value={role.id}>
                                            {role.tenNhom}
                                        </MenuItem>
                                    ))}
                                </Select>
                            </FormControl>
                        </div>
                        <div className="flex flex-col items-center gap-3">
                            <label className="cursor-pointer bg-blue-500 text-white px-3 py-1.5 text-sm rounded hover:bg-blue-600">
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
                                    className="w-64 h-48 rounded shadow-md"
                                />
                            )}
                            <div className="w-full flex flex-row gap-3 mt-2">
                                <button
                                    className="w-1/2 bg-gray-500 text-white py-1.5 text-sm rounded hover:bg-gray-600 transition-colors"
                                    onClick={closeAddModal}
                                >
                                    Hủy
                                </button>
                                <button
                                    className="w-1/2 bg-blue-600 text-white py-1.5 text-sm rounded hover:bg-blue-700 transition-colors"
                                    onClick={handleUpload}
                                    disabled={image === null}
                                >
                                    {isLoading ? 'Loading...' : 'Lưu'}
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            )}

            {/* Add/Edit Role Modal */}
            {showRoleModal && (
                <div className="fixed inset-0 z-50 flex items-center justify-center">
                    <div
                        className="absolute inset-0 bg-black bg-opacity-60"
                        onClick={() => setShowRoleModal(false)}
                    ></div>
                    <div className="bg-white z-50 shadow-md shadow-black px-5 py-4 rounded-md relative w-full max-w-sm">
                        <h2 className="font-semibold text-lg mb-2 text-blue-900">
                            {isEditRole ? 'Chỉnh sửa nhóm' : 'Thêm mới nhóm'}
                        </h2>
                        <form onSubmit={handleRoleSubmit}>
                            <div className="mb-2">
                                <TextField
                                    label="Tên nhóm"
                                    variant="outlined"
                                    fullWidth
                                    name="roleName"
                                    value={roleFormData.roleName}
                                    onChange={handleRoleInputChange}
                                    margin="dense"
                                    required
                                    size="small"
                                />
                            </div>
                            <div className="flex flex-row gap-3 mt-2">
                                <button
                                    type="button"
                                    className="w-1/2 bg-gray-500 text-white py-1.5 text-sm rounded hover:bg-gray-600 transition-colors"
                                    onClick={() => setShowRoleModal(false)}
                                >
                                    Hủy
                                </button>
                                <button
                                    type="submit"
                                    className="w-1/2 bg-blue-600 text-white py-1.5 text-sm rounded hover:bg-blue-700 transition-colors"
                                    disabled={!roleFormData.roleName}
                                >
                                    {isEditRole ? 'Lưu' : 'Thêm'}
                                </button>
                            </div>
                        </form>
                    </div>
                </div>
            )}

            <Box sx={{ width: '100%' }}>
                <Box sx={{ borderBottom: 1, borderColor: 'divider' }}>
                    <Tabs
                        value={value}
                        onChange={handleChange}
                        aria-label="basic tabs example"
                    >
                        <Tab label="Thông tin người dùng" {...a11yProps(0)} />
                        <Tab label="Phân nhóm" {...a11yProps(1)} />
                    </Tabs>
                    <div className="py-4">
                        <Button
                            variant="contained"
                            color="primary"
                            size="small"
                            style={{ marginRight: 8 }}
                            startIcon={<GridAddIcon />}
                            onClick={openAddModal}
                        >
                            Thêm mới nhân viên
                        </Button>
                        <Button
                            variant="contained"
                            color="secondary"
                            size="small"
                            style={{ marginRight: 8 }}
                            startIcon={<GridAddIcon />}
                            onClick={() => {
                                setIsEditRole(false);
                                setRoleFormData({ id: '', roleName: '' });
                                setShowRoleModal(true);
                            }}
                        >
                            Thêm mới phân nhóm
                        </Button>
                    </div>
                </Box>
                <CustomTabPanel value={value} index={0}>
                    <Paper sx={{ width: '100%' }}>
                        <DataGrid
                            rows={formattedPersonnels}
                            columns={columns}
                            getRowId={(row) => row.id}
                            initialState={{ pagination: { paginationModel } }}
                            pageSizeOptions={[5, 10]}
                            checkboxSelection={false}
                            disableRowSelectionOnClick
                            disableDensitySelector
                            sx={{ border: 0 }}
                            slots={{
                                noRowsOverlay: () => (
                                    <div className="flex justify-center items-center h-full">
                                        <div className="flex flex-col items-center">
                                            <GridVisibilityOffIcon
                                                sx={{
                                                    fontSize: 40,
                                                    color: 'gray',
                                                }}
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
                </CustomTabPanel>
                <CustomTabPanel value={value} index={1}>
                    <Paper sx={{ width: '80%' }}>
                        <DataGrid
                            rows={formattedGroups}
                            columns={columnsPhanNhom}
                            getRowId={(row) => row.id}
                            initialState={{ pagination: { paginationModel } }}
                            pageSizeOptions={[5, 10]}
                            checkboxSelection={false}
                            disableRowSelectionOnClick
                            disableDensitySelector
                            sx={{ border: 0 }}
                            slots={{
                                noRowsOverlay: () => (
                                    <div className="flex justify-center items-center h-full">
                                        <div className="flex flex-col items-center">
                                            <GridVisibilityOffIcon
                                                sx={{
                                                    fontSize: 40,
                                                    color: 'gray',
                                                }}
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
                </CustomTabPanel>
            </Box>
        </>
    );
};

export default List;
