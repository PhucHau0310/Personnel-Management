'use client';

import axios from 'axios';
import { deleteCookie, getCookie, setCookie } from 'cookies-next';
import { jwtDecode, JwtPayload } from 'jwt-decode';

const axiosJwt = axios.create({
    baseURL: 'https://localhost:3031/api',
});

// Flag để theo dõi trạng thái refresh token
let isRefreshing = false;
// Hàng đợi để lưu các request đang đợi token mới
let refreshSubscribers: ((token: string) => void)[] = [];

// Hàm để đăng ký các request đang chờ token mới
const subscribeTokenRefresh = (cb: (token: string) => void) => {
    refreshSubscribers.push(cb);
};

// Hàm để thông báo cho các request đang chờ sau khi có token mới
const onRefreshed = (token: string) => {
    refreshSubscribers.forEach((cb) => cb(token));
    refreshSubscribers = [];
};

const refreshAccessToken = async () => {
    console.log('Bắt đầu refresh token');
    try {
        if (isRefreshing) {
            // Nếu đang refresh, trả về một Promise sẽ được giải quyết khi token mới có sẵn
            return new Promise<string | null>((resolve) => {
                subscribeTokenRefresh((token) => {
                    resolve(token);
                });
            });
        }

        isRefreshing = true;
        const accessToken = getCookie('access-token');
        const refreshToken = getCookie('refresh-token');

        if (!accessToken || !refreshToken) {
            isRefreshing = false;
            return null;
        }

        const res = await axios.post(
            'https://localhost:3031/api/auth/refresh-token',
            {
                accessToken: accessToken,
                refreshToken: refreshToken,
            }
        );

        const result = await res.data;

        if (res.status === 200) {
            deleteCookie('access-token');
            deleteCookie('refresh-token');

            setCookie('access-token', result.data.accessToken);
            setCookie('refresh-token', result.data.refreshToken);

            const newToken = result.data.accessToken;
            onRefreshed(newToken);
            isRefreshing = false;
            return newToken;
        } else {
            isRefreshing = false;
            throw new Error('Failed to refresh token');
        }
    } catch (error) {
        console.log('Failed to refresh token', error);
        isRefreshing = false;

        // Xóa token cũ khi refresh thất bại
        deleteCookie('access-token');
        deleteCookie('refresh-token');

        // Nếu có trình xử lý lỗi toàn cục, có thể gọi nó ở đây
        // (ví dụ: đăng xuất người dùng và chuyển hướng đến trang đăng nhập)

        return null;
    }
};

axiosJwt.interceptors.request.use(
    async (config) => {
        let accessToken = getCookie('access-token');
        if (accessToken) {
            try {
                const decodedToken = jwtDecode<JwtPayload>(
                    accessToken.toString()
                );

                let currentTime = Date.now() / 1000;

                if (decodedToken.exp && decodedToken.exp < currentTime) {
                    console.log('Gọi hàm refresh token ....');
                    // Token đã hết hạn, cần refresh
                    accessToken = await refreshAccessToken();
                    if (!accessToken) {
                        // Nếu không thể refresh token, có thể chuyển hướng đến trang đăng nhập
                        deleteCookie('access-token');
                        deleteCookie('refresh-token');
                        localStorage.removeItem('username');
                        window.location.reload();
                        return Promise.reject('Authentication failed');
                    }
                }

                console.log('Gán token vào header');
                // Luôn cập nhật header với token mới nhất
                config.headers['Authorization'] = `Bearer ${accessToken}`;
            } catch (error) {
                console.error('Error decoding token:', error);
                // Nếu có lỗi khi giải mã token, xóa token và buộc người dùng đăng nhập lại

                deleteCookie('access-token');
                deleteCookie('refresh-token');
                localStorage.removeItem('username');
                window.location.reload();
                return Promise.reject('Authentication failed');
            }
        }
        return config;
    },
    (error) => {
        return Promise.reject(error);
    }
);

// Response Interceptor
export const setupAxiosInterceptors = (
    showAlert: (
        msg: string,
        level: 'error' | 'success' | 'info' | 'warning'
    ) => void
) => {
    axiosJwt.interceptors.response.use(
        (response) => response,
        async (error) => {
            const originalRequest = error.config;

            // Nếu lỗi 401 (Unauthorized) và chưa thử refresh token
            if (error.response?.status === 401 && !originalRequest._retry) {
                originalRequest._retry = true; // Đánh dấu request này đã thử refresh

                try {
                    const newToken = await refreshAccessToken();
                    if (newToken) {
                        // Cập nhật token trong request gốc và thử lại
                        originalRequest.headers['Authorization'] =
                            `Bearer ${newToken}`;
                        return axiosJwt(originalRequest);
                    } else {
                        // Không thể refresh token
                        showAlert(
                            'Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.',
                            'error'
                        );
                        deleteCookie('access-token');
                        deleteCookie('refresh-token');
                        localStorage.removeItem('username');
                        window.location.reload();
                        return Promise.reject(error);
                    }
                } catch (refreshError) {
                    showAlert(
                        'Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.',
                        'error'
                    );
                    deleteCookie('access-token');
                    deleteCookie('refresh-token');
                    localStorage.removeItem('username');
                    window.location.reload();
                    return Promise.reject(error);
                }
            }

            if (error.response?.status === 403) {
                showAlert('Bạn không có quyền truy cập.', 'error');
            }

            return Promise.reject(error);
        }
    );
};

export default axiosJwt;
