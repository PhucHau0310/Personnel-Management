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
    Checkbox,
    FormControl,
    IconButton,
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

interface AccountFormData {
    id: string;
    username: string;
    roleAuthorization: string;
}

interface NewAccountFormData {
    username: string;
    password: string;
    roleAuthorization: string;
}

interface RoleFormData {
    id: string;
    roleName: string;
    permissions: string[];
}

interface NewRoleFormData {
    roleName: string;
    permissions: string[];
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

const Setting = () => {
    const [value, setValue] = React.useState(0);
    const [formattedAccounts, setFormattedAccounts] = React.useState<any>([]);
    const [formattedRoles, setFormattedRoles] = React.useState<any>([]);
    const [formattedPermissions, setFormattedPermissions] = React.useState<any>(
        []
    );

    // Delete account management states
    const [showDeleteModal, setShowDeleteModal] = React.useState(false);
    const [accountToDelete, setAccountToDelete] = React.useState<string | null>(
        null
    );

    // Edit account management states
    const [showEditModal, setShowEditModal] = React.useState(false);
    const [accountFormData, setAccountFormData] =
        React.useState<AccountFormData>({
            id: '',
            username: '',
            roleAuthorization: '',
        });

    // Add account management states
    const [showAddModal, setShowAddModal] = React.useState(false);
    const [newAccountFormData, setNewAccountFormData] =
        React.useState<NewAccountFormData>({
            username: '',
            password: '',
            roleAuthorization: '',
        });
    const [showPassword, setShowPassword] = React.useState(false);

    // Role management states
    const [showDeleteRoleModal, setShowDeleteRoleModal] = React.useState(false);
    const [roleToDelete, setRoleToDelete] = React.useState<string | null>(null);

    const [showEditRoleModal, setShowEditRoleModal] = React.useState(false);
    const [roleFormData, setRoleFormData] = React.useState<RoleFormData>({
        id: '',
        roleName: '',
        permissions: [],
    });

    const [showAddRoleModal, setShowAddRoleModal] = React.useState(false);
    const [newRoleFormData, setNewRoleFormData] =
        React.useState<NewRoleFormData>({
            roleName: '',
            permissions: [],
        });

    const { showAlert, AlertComponent } = useAlert();

    const handleChange = (event: React.SyntheticEvent, newValue: number) => {
        setValue(newValue);
    };

    const handleEdit = (rowAccount: any) => {
        const roleId = formattedRoles.find(
            (role: any) => role.tenNhomPhanQuyen === rowAccount.nhomPhanQuyen
        ).id;

        setAccountFormData({
            id: rowAccount.id,
            username: rowAccount.tenTaiKhoan,
            roleAuthorization: roleId,
        });
        setShowEditModal(true);
    };

    const handleDelete = (id: string) => {
        setAccountToDelete(id);
        setShowDeleteModal(true);
    };

    const confirmDelete = async () => {
        if (!accountToDelete) return;

        try {
            const response = await axiosJwt.delete(
                `/account/${accountToDelete}`
            );

            if (response.status === 200) {
                // Remove the deleted account from the list
                setFormattedAccounts(
                    formattedAccounts.filter(
                        (account: any) => account.id !== accountToDelete
                    )
                );
                setShowDeleteModal(false);
                setAccountToDelete(null);
                showAlert('Tài khoản đã được xóa thành công.', 'success');
            } else {
                showAlert('Lỗi khi xóa tài khoản', 'error');
            }
        } catch (error) {
            console.error('Lỗi khi xóa tài khoản:', error);
        }
    };

    const handleInputChange = (
        event:
            | React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>
            | SelectChangeEvent<unknown>
    ) => {
        const { name, value } = event.target;
        setAccountFormData((prev) => ({
            ...prev,
            [name as string]: value,
        }));
    };

    const handleNewAccountInputChange = (
        event:
            | React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>
            | SelectChangeEvent<unknown>
    ) => {
        const { name, value } = event.target;
        setNewAccountFormData((prev) => ({
            ...prev,
            [name as string]: value,
        }));
    };

    // Role input handlers
    const handleRoleInputChange = (
        event: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>
    ) => {
        const { name, value } = event.target;
        setRoleFormData((prev) => ({
            ...prev,
            [name]: value,
        }));
    };

    const handleEditPermissionsChange = (
        event: SelectChangeEvent<string[]>
    ) => {
        const { value } = event.target;
        setRoleFormData((prev) => ({
            ...prev,
            permissions: typeof value === 'string' ? value.split(',') : value,
        }));
    };

    const handleNewRoleInputChange = (
        event: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>
    ) => {
        const { name, value } = event.target;
        setNewRoleFormData((prev) => ({
            ...prev,
            [name]: value,
        }));
    };

    const handlePermissionsChange = (event: SelectChangeEvent<string[]>) => {
        const { value } = event.target;
        setNewRoleFormData((prev) => ({
            ...prev,
            permissions: typeof value === 'string' ? value.split(',') : value,
        }));
    };

    // Role submission handlers
    const handleSubmitEditRole = async (e: React.FormEvent) => {
        e.preventDefault();

        try {
            const response = await axiosJwt.put(
                `/account/roles/${roleFormData.id}`,
                {
                    roleName: roleFormData.roleName,
                    permissionIds: roleFormData.permissions,
                }
            );

            if (response.status === 200) {
                const updatedRoles = await fetchRoleAccounts();
                await fetchAccounts(updatedRoles);
                setShowEditRoleModal(false);
                showAlert('Nhóm quyền đã được cập nhật thành công.', 'success');
            } else {
                showAlert('Lỗi khi cập nhật nhóm quyền.', 'error');
            }
        } catch (error) {
            console.error('Lỗi khi cập nhật nhóm quyền:', error);
            showAlert('Lỗi khi cập nhật nhóm quyền.', 'error');
        }
    };

    const handleSubmitAddRole = async (e: React.FormEvent) => {
        e.preventDefault();

        try {
            const response = await axiosJwt.post(`/account/roles`, {
                roleName: newRoleFormData.roleName,
                permissionIds: newRoleFormData.permissions,
            });

            if (response.status === 200) {
                const updatedRoles = await fetchRoleAccounts();
                await fetchAccounts(updatedRoles);
                setShowAddRoleModal(false);
                resetNewRoleForm();
                showAlert('Nhóm quyền đã được tạo thành công.', 'success');
            } else {
                showAlert('Lỗi khi tạo nhóm quyền.', 'error');
            }
        } catch (error) {
            console.error('Lỗi khi tạo nhóm quyền:', error);
            showAlert('Lỗi khi tạo nhóm quyền.', 'error');
        }
    };

    const handleEditRole = (roleData: any) => {
        console.log({ roleData });
        setRoleFormData({
            id: roleData.id,
            roleName: roleData.tenNhomPhanQuyen,
            // permissions: roleData.danhSachQuyen,
            permissions: roleData.danhSachQuyen.map((perm: any) => perm.id),
        });
        setShowEditRoleModal(true);
    };

    const handleDeleteRole = (id: string) => {
        setRoleToDelete(id);
        setShowDeleteRoleModal(true);
    };

    const confirmDeleteRole = async () => {
        if (!roleToDelete) return;

        try {
            const response = await axiosJwt.delete(
                `/account/roles/${roleToDelete}`
            );

            if (response.status === 200) {
                // Remove the deleted role from the list
                setFormattedRoles(
                    formattedRoles.filter(
                        (role: any) => role.id !== roleToDelete
                    )
                );
                // Refresh accounts to ensure consistent data
                await fetchAccounts(
                    formattedRoles.filter(
                        (role: any) => role.id !== roleToDelete
                    )
                );
                setShowDeleteRoleModal(false);
                setRoleToDelete(null);
                showAlert('Nhóm quyền đã được xóa thành công.', 'success');
            } else {
                showAlert('Lỗi khi xóa nhóm quyền', 'error');
            }
        } catch (error) {
            console.error('Lỗi khi xóa nhóm quyền:', error);
            showAlert(
                'Lỗi khi xóa nhóm quyền. Nhóm quyền có thể đang được sử dụng bởi tài khoản.',
                'error'
            );
        }
    };

    // Role modal handlers
    const closeDeleteRoleModal = () => {
        setShowDeleteRoleModal(false);
        setRoleToDelete(null);
    };

    const closeEditRoleModal = () => {
        setShowEditRoleModal(false);
    };

    const openAddRoleModal = () => {
        setShowAddRoleModal(true);
    };

    const closeAddRoleModal = () => {
        setShowAddRoleModal(false);
        resetNewRoleForm();
    };

    const resetNewRoleForm = () => {
        setNewRoleFormData({
            roleName: '',
            permissions: [],
        });
    };

    const handleSubmitEdit = async (e: React.FormEvent) => {
        e.preventDefault();

        try {
            const response = await axiosJwt.put(
                `/account/${accountFormData.id}`,
                {
                    username: accountFormData.username,
                    roleAccountId: accountFormData.roleAuthorization,
                }
            );

            if (response.status === 200) {
                await fetchAccounts(formattedRoles);
                setShowEditModal(false);
                showAlert('Tài khoản đã được cập nhật thành công.', 'success');
            } else {
                showAlert('Lỗi khi cập nhật tài khoản.', 'error');
            }
        } catch (error) {
            console.error('Lỗi khi cập nhật tài khoản:', error);
        }
    };

    const handleSubmitAdd = async (e: React.FormEvent) => {
        e.preventDefault();

        try {
            const response = await axiosJwt.post(`/account`, {
                username: newAccountFormData.username,
                password: newAccountFormData.password,
                roleId: newAccountFormData.roleAuthorization,
            });

            if (response.status === 200) {
                await fetchAccounts(formattedRoles);
                setShowAddModal(false);
                resetNewAccountForm();
                showAlert('Tài khoản đã được tạo thành công.', 'success');
            } else {
                showAlert('Lỗi khi tạo tài khoản.', 'error');
            }
        } catch (error) {
            console.error('Lỗi khi tạo tài khoản:', error);
        }
    };

    const closeDeleteModal = () => {
        setShowDeleteModal(false);
        setAccountToDelete(null);
    };

    const closeEditModal = () => {
        setShowEditModal(false);
    };

    const openAddModal = () => {
        setShowAddModal(true);
    };

    const closeAddModal = () => {
        setShowAddModal(false);
        resetNewAccountForm();
    };

    const resetNewAccountForm = () => {
        setNewAccountFormData({
            username: '',
            password: '',
            roleAuthorization: '',
        });
        setShowPassword(false);
    };

    const toggleShowPassword = () => {
        setShowPassword(!showPassword);
    };

    const fetchAccounts = async (roles: any) => {
        try {
            const res = await axiosJwt.get('/account', {
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                },
            });

            let accountsData = res.data.data;
            if (accountsData.$values) {
                accountsData = accountsData.$values;
            }

            const accounts = Array.isArray(accountsData) ? accountsData : [];

            const formattedData = accounts.map(
                (account: any, index: number) => ({
                    stt: index + 1,
                    id: account.id,
                    tenTaiKhoan: account.username,
                    nhomPhanQuyen:
                        roles.find(
                            (role: any) => role.id === account.roleAccountId
                        )?.tenNhomPhanQuyen || 'Không xác định',

                    ngayTao: account.createdAt,
                    ngayCapNhat: account.updatedAt,
                })
            );
            setFormattedAccounts(formattedData);
        } catch (error) {
            console.log(error);
            showAlert('Lỗi trong quá trình lấy danh sách tài khoản', 'error');
        }
    };

    const fetchRoleAccounts = async () => {
        try {
            const res = await axiosJwt.get('/account/roles', {
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                },
            });

            let rolesData = await res.data.data;
            if (rolesData.$values) {
                rolesData = rolesData.$values;
            }

            const roles = Array.isArray(rolesData) ? rolesData : [];

            const roleData = roles.map((role: any, index: number) => ({
                stt: index + 1,
                id: role.id,
                tenNhomPhanQuyen: role.roleName,
                ngayTao: role.createdAt,
                ngayCapNhat: role.updatedAt,
                danhSachQuyen:
                    role.permissions && role.permissions.$values
                        ? role.permissions.$values
                        : Array.isArray(role.permissions)
                          ? role.permissions
                          : [],
            }));

            setFormattedRoles(roleData);
            return roleData;
        } catch (error) {
            showAlert('Lỗi trong quá trình get roles', 'error');
            return [];
        }
    };

    const fetchPermissions = async () => {
        try {
            const res = await axiosJwt.get('/account/permissions', {
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                },
            });

            console.log(res);

            let permissionsData = await res.data.data;
            if (permissionsData.$values) {
                permissionsData = permissionsData.$values;
            }

            const permissions = Array.isArray(permissionsData)
                ? permissionsData
                : [];

            setFormattedPermissions(permissions);
        } catch (error) {
            showAlert('Lỗi trong quá trình get roles', 'error');
        }
    };

    React.useEffect(() => {
        const fetchData = async () => {
            const roles = await fetchRoleAccounts();
            await fetchAccounts(roles);
            await fetchPermissions();
        };

        fetchData();
    }, []);

    const columns: GridColDef[] = [
        { field: 'stt', headerName: 'STT', width: 70 },
        { field: 'id', headerName: 'ID', width: 180 },
        { field: 'tenTaiKhoan', headerName: 'Tên tài khoản', width: 130 },
        { field: 'nhomPhanQuyen', headerName: 'Nhóm phân quyền', width: 150 },
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
                row.ngayCapNhat !== null
                    ? new Date(row.ngayCapNhat)
                    : new Date('...'),
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
        { field: 'stt', headerName: 'STT', width: 200 },
        {
            field: 'tenNhomPhanQuyen',
            headerName: 'Tên nhóm phân quyền',
            width: 200,
        },
        {
            field: 'ngayTao',
            headerName: 'Ngày tạo',
            type: 'dateTime',
            width: 170,
            valueGetter: (value, row) => new Date(row.ngayTao),
        },
        {
            field: 'ngayCapNhat',
            headerName: 'Ngày cập nhật',
            type: 'dateTime',
            width: 170,
            valueGetter: (value, row) =>
                row.ngayCapNhat !== null ? new Date(row.ngayCapNhat) : 'NULL',
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
                        onClick={() => handleEditRole(params.row)}
                        style={{ marginRight: 8 }}
                    >
                        Chỉnh sửa
                    </Button>
                    <Button
                        variant="contained"
                        color="secondary"
                        size="small"
                        onClick={() => handleDeleteRole(params.row.id)}
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
                    {/* Overlay with black semi-transparent background */}
                    <div
                        className="absolute inset-0 bg-black bg-opacity-60"
                        onClick={closeDeleteModal}
                    ></div>

                    {/* Modal Content */}
                    <div className="bg-white z-50 shadow-md shadow-black px-10 py-6 rounded-md relative">
                        <h2 className="font-semibold text-xl mb-2 text-blue-900">
                            Bạn có chắc chắn muốn xóa tài khoản
                        </h2>

                        <p className="text-center mb-6">
                            ID: {accountToDelete}
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

            {/* Edit Account Modal */}
            {showEditModal && (
                <div className="fixed inset-0 z-50 flex items-center justify-center">
                    {/* Overlay with black semi-transparent background */}
                    <div
                        className="absolute inset-0 bg-black bg-opacity-60"
                        onClick={closeEditModal}
                    ></div>

                    {/* Modal Content */}
                    <div className="bg-white z-50 shadow-md shadow-black px-8 py-6 rounded-md relative w-full max-w-md">
                        <h2 className="font-semibold text-xl mb-4 text-blue-900">
                            Chỉnh sửa tài khoản
                        </h2>

                        <form onSubmit={handleSubmitEdit}>
                            <div className="mb-4">
                                <TextField
                                    label="ID"
                                    variant="outlined"
                                    fullWidth
                                    value={accountFormData.id}
                                    disabled
                                    margin="normal"
                                />
                            </div>

                            <div className="mb-4">
                                <TextField
                                    label="Tên tài khoản"
                                    variant="outlined"
                                    fullWidth
                                    name="username"
                                    value={accountFormData.username}
                                    onChange={handleInputChange}
                                    margin="normal"
                                    required
                                />
                            </div>

                            <div className="mb-6">
                                <FormControl fullWidth margin="normal">
                                    <InputLabel id="role-select-label">
                                        Nhóm phân quyền
                                    </InputLabel>
                                    <Select
                                        labelId="role-select-label"
                                        id="role-select"
                                        name="roleAuthorization"
                                        value={
                                            accountFormData.roleAuthorization
                                        }
                                        label="Nhóm phân quyền"
                                        onChange={handleInputChange}
                                    >
                                        {formattedRoles.map(
                                            (role: any, index: number) => (
                                                <MenuItem
                                                    key={index}
                                                    value={role.id}
                                                >
                                                    {role.tenNhomPhanQuyen}
                                                </MenuItem>
                                            )
                                        )}
                                    </Select>
                                </FormControl>
                            </div>

                            <div className="flex flex-row items-center gap-4">
                                <button
                                    type="button"
                                    className="w-1/2 bg-gray-500 text-white py-2 rounded-md hover:bg-gray-600 transition-colors"
                                    onClick={closeEditModal}
                                >
                                    Hủy
                                </button>
                                <button
                                    type="submit"
                                    className="w-1/2 bg-blue-600 text-white py-2 rounded-md hover:bg-blue-700 transition-colors"
                                >
                                    Cập nhật
                                </button>
                            </div>
                        </form>
                    </div>
                </div>
            )}

            {/* Add Account Modal */}
            {showAddModal && (
                <div className="fixed inset-0 z-50 flex items-center justify-center">
                    {/* Overlay */}
                    <div
                        className="absolute inset-0 bg-black bg-opacity-60"
                        onClick={closeAddModal}
                    ></div>

                    {/* Modal Content */}
                    <div className="bg-white z-50 shadow-md shadow-black px-8 py-6 rounded-md relative w-full max-w-md">
                        <h2 className="font-semibold text-xl mb-4 text-blue-900">
                            Thêm mới tài khoản
                        </h2>

                        <form onSubmit={handleSubmitAdd}>
                            <div className="mb-4">
                                <TextField
                                    label="Tên tài khoản"
                                    variant="outlined"
                                    fullWidth
                                    name="username"
                                    value={newAccountFormData.username}
                                    onChange={handleNewAccountInputChange}
                                    margin="normal"
                                    required
                                    autoFocus
                                />
                            </div>

                            <div className="mb-4">
                                <TextField
                                    label="Mật khẩu"
                                    variant="outlined"
                                    fullWidth
                                    name="password"
                                    type={showPassword ? 'text' : 'password'}
                                    value={newAccountFormData.password}
                                    onChange={handleNewAccountInputChange}
                                    margin="normal"
                                    required
                                    InputProps={{
                                        endAdornment: (
                                            <IconButton
                                                aria-label="toggle password visibility"
                                                onClick={toggleShowPassword}
                                                edge="end"
                                            >
                                                {showPassword ? (
                                                    <GridVisibilityOffIcon />
                                                ) : (
                                                    <GridVisibilityOffIcon />
                                                )}
                                            </IconButton>
                                        ),
                                    }}
                                />
                            </div>

                            <div className="mb-6">
                                <FormControl fullWidth margin="normal">
                                    <InputLabel id="new-role-select-label">
                                        Nhóm phân quyền
                                    </InputLabel>
                                    <Select
                                        labelId="new-role-select-label"
                                        id="new-role-select"
                                        name="roleAuthorization"
                                        value={
                                            newAccountFormData.roleAuthorization
                                        }
                                        label="Nhóm phân quyền"
                                        onChange={handleNewAccountInputChange}
                                    >
                                        {formattedRoles.map(
                                            (role: any, index: number) => (
                                                <MenuItem
                                                    key={index}
                                                    value={role.id}
                                                >
                                                    {role.tenNhomPhanQuyen}
                                                </MenuItem>
                                            )
                                        )}
                                    </Select>
                                </FormControl>
                            </div>

                            <div className="flex flex-row items-center gap-4">
                                <button
                                    type="button"
                                    className="w-1/2 bg-gray-500 text-white py-2 rounded-md hover:bg-gray-600 transition-colors"
                                    onClick={closeAddModal}
                                >
                                    Hủy
                                </button>
                                <button
                                    type="submit"
                                    className="w-1/2 bg-blue-600 text-white py-2 rounded-md hover:bg-blue-700 transition-colors"
                                    disabled={
                                        !newAccountFormData.username ||
                                        !newAccountFormData.password ||
                                        !newAccountFormData.roleAuthorization
                                    }
                                >
                                    Lưu
                                </button>
                            </div>
                        </form>
                    </div>
                </div>
            )}

            {/* Delete Role Confirmation Modal */}
            {showDeleteRoleModal && (
                <div className="fixed inset-0 z-50 flex items-center justify-center">
                    <div
                        className="absolute inset-0 bg-black bg-opacity-60"
                        onClick={closeDeleteRoleModal}
                    ></div>

                    <div className="bg-white z-50 shadow-md shadow-black px-10 py-6 rounded-md relative">
                        <h2 className="font-semibold text-xl mb-2 text-blue-900">
                            Bạn có chắc chắn muốn xóa nhóm quyền này?
                        </h2>

                        <p className="text-center mb-6">ID: {roleToDelete}</p>

                        <p className="text-yellow-600 text-sm mb-4">
                            Lưu ý: Xóa nhóm quyền sẽ ảnh hưởng đến các tài khoản
                            được gán quyền này.
                        </p>

                        <div className="flex flex-row items-center gap-4 text-white">
                            <button
                                className="w-1/2 bg-gray-500 py-2 rounded-md hover:bg-gray-600 transition-colors"
                                onClick={closeDeleteRoleModal}
                            >
                                Hủy
                            </button>
                            <button
                                className="w-1/2 bg-blue-600 py-2 rounded-md hover:bg-blue-700 transition-colors"
                                onClick={confirmDeleteRole}
                            >
                                Xác nhận
                            </button>
                        </div>
                    </div>
                </div>
            )}

            {/* Add Role Account Modal */}
            {showAddRoleModal && (
                <div className="fixed inset-0 z-50 flex items-center justify-center">
                    {/* Overlay */}
                    <div
                        className="absolute inset-0 bg-black bg-opacity-60"
                        onClick={closeAddRoleModal}
                    ></div>

                    {/* Modal Content */}
                    <div className="bg-white z-50 shadow-md shadow-black px-8 py-6 rounded-md relative w-full max-w-md">
                        <h2 className="font-semibold text-xl mb-4 text-blue-900">
                            Thêm mới nhóm quyền
                        </h2>

                        <form onSubmit={handleSubmitAddRole}>
                            <div className="mb-4">
                                <TextField
                                    label="Tên nhóm quyền"
                                    variant="outlined"
                                    fullWidth
                                    name="roleName"
                                    value={newRoleFormData.roleName}
                                    onChange={handleNewRoleInputChange}
                                    margin="normal"
                                    required
                                    autoFocus
                                />
                            </div>

                            <div className="mb-6">
                                <FormControl fullWidth margin="normal">
                                    <InputLabel id="new-role-select-label">
                                        Quyền hạn
                                    </InputLabel>
                                    <Select
                                        labelId="new-role-select-label"
                                        id="new-role-select"
                                        name="permissions"
                                        multiple // Cho phép chọn nhiều giá trị
                                        value={newRoleFormData.permissions} // Phải là một mảng
                                        onChange={handlePermissionsChange}
                                        renderValue={
                                            (selected) =>
                                                formattedPermissions
                                                    .filter((perm: any) =>
                                                        selected.includes(
                                                            perm.id
                                                        )
                                                    )
                                                    .map(
                                                        (perm: any) => perm.name
                                                    )
                                                    .join(', ') // Hiển thị danh sách quyền hạn đã chọn
                                        }
                                    >
                                        {formattedPermissions.map(
                                            (
                                                permission: any,
                                                index: number
                                            ) => (
                                                <MenuItem
                                                    key={index}
                                                    value={permission.id}
                                                >
                                                    <Checkbox
                                                        checked={newRoleFormData.permissions.includes(
                                                            permission.id
                                                        )}
                                                    />
                                                    {permission.name}
                                                </MenuItem>
                                            )
                                        )}
                                    </Select>
                                </FormControl>
                            </div>

                            <div className="flex flex-row items-center gap-4">
                                <button
                                    type="button"
                                    className="w-1/2 bg-gray-500 text-white py-2 rounded-md hover:bg-gray-600 transition-colors"
                                    onClick={closeAddRoleModal}
                                >
                                    Hủy
                                </button>
                                <button
                                    type="submit"
                                    className="w-1/2 bg-blue-600 text-white py-2 rounded-md hover:bg-blue-700 transition-colors"
                                    disabled={!newRoleFormData.roleName}
                                >
                                    Lưu
                                </button>
                            </div>
                        </form>
                    </div>
                </div>
            )}

            {/* Edit Account Modal */}
            {showEditRoleModal && (
                <div className="fixed inset-0 z-50 flex items-center justify-center">
                    {/* Overlay with black semi-transparent background */}
                    <div
                        className="absolute inset-0 bg-black bg-opacity-60"
                        onClick={closeEditRoleModal}
                    ></div>

                    {/* Modal Content */}
                    <div className="bg-white z-50 shadow-md shadow-black px-8 py-6 rounded-md relative w-full max-w-md">
                        <h2 className="font-semibold text-xl mb-4 text-blue-900">
                            Chỉnh sửa nhóm quyền
                        </h2>

                        <form onSubmit={handleSubmitEditRole}>
                            <div className="mb-4">
                                <TextField
                                    label="ID"
                                    variant="outlined"
                                    fullWidth
                                    value={roleFormData.id}
                                    disabled
                                    margin="normal"
                                />
                            </div>

                            <div className="mb-4">
                                <TextField
                                    label="Tên nhóm quyền"
                                    variant="outlined"
                                    fullWidth
                                    name="roleName"
                                    value={roleFormData.roleName}
                                    onChange={handleRoleInputChange}
                                    margin="normal"
                                    required
                                />
                            </div>

                            <div className="mb-6">
                                <FormControl fullWidth margin="normal">
                                    <InputLabel id="role-select-label">
                                        Quyền hạn
                                    </InputLabel>
                                    <Select
                                        labelId="role-select-label"
                                        id="edit-role-select"
                                        name="permissions"
                                        multiple // Cho phép chọn nhiều giá trị
                                        value={roleFormData.permissions} // Phải là một mảng
                                        onChange={handleEditPermissionsChange}
                                        renderValue={
                                            (selected) =>
                                                formattedPermissions
                                                    .filter((perm: any) =>
                                                        selected.includes(
                                                            perm.id
                                                        )
                                                    )
                                                    .map(
                                                        (perm: any) => perm.name
                                                    )
                                                    .join(', ') // Hiển thị danh sách quyền hạn đã chọn
                                        }
                                    >
                                        {formattedPermissions.map(
                                            (
                                                permission: any,
                                                index: number
                                            ) => (
                                                <MenuItem
                                                    key={index}
                                                    value={permission.id}
                                                >
                                                    <Checkbox
                                                        checked={roleFormData.permissions.includes(
                                                            permission.id
                                                        )}
                                                    />
                                                    {permission.name}
                                                </MenuItem>
                                            )
                                        )}
                                    </Select>
                                </FormControl>
                            </div>

                            <div className="flex flex-row items-center gap-4">
                                <button
                                    type="button"
                                    className="w-1/2 bg-gray-500 text-white py-2 rounded-md hover:bg-gray-600 transition-colors"
                                    onClick={closeEditRoleModal}
                                >
                                    Hủy
                                </button>
                                <button
                                    type="submit"
                                    className="w-1/2 bg-blue-600 text-white py-2 rounded-md hover:bg-blue-700 transition-colors"
                                >
                                    Cập nhật
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
                        <Tab label="Danh sách tài khoản" {...a11yProps(0)} />
                        <Tab label="Nhóm quyền" {...a11yProps(1)} />
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
                            Thêm mới tài khoản
                        </Button>

                        <Button
                            variant="contained"
                            color="secondary"
                            size="small"
                            style={{ marginRight: 8 }}
                            startIcon={<GridAddIcon />}
                            onClick={openAddRoleModal}
                        >
                            Thêm mới nhóm quyền
                        </Button>
                    </div>
                </Box>
                <CustomTabPanel value={value} index={0}>
                    <Paper sx={{ width: '95%' }}>
                        <DataGrid
                            rows={formattedAccounts}
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
                            rows={formattedRoles}
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

export default Setting;
