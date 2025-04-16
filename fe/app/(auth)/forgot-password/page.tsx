'use client';

import useAlert from '@/hooks/useAlert';
import { faKey, faL, faMailBulk } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import axios from 'axios';
import React from 'react';

const ForgotPassword = () => {
    const [username, setUsername] = React.useState('');
    const [email, setEmail] = React.useState('');
    const [turn, setTurn] = React.useState(0);
    const [code, setCode] = React.useState('');
    const [newPass, setNewPass] = React.useState('');
    const [loading, setLoading] = React.useState(false);
    const { showAlert, AlertComponent } = useAlert();

    const handleResetPass = async (e: any) => {
        e.preventDefault();
        setLoading(true);
        try {
            const res = await axios.post(
                `https://localhost:3031/api/account/forgot-password?email=${email}&username=${username}`
            );

            if (res.status === 200) {
                showAlert('Đã gửi mã xác thực vào email', 'success');
                setTurn(1);
            } else {
                showAlert('Lỗi khi gửi mã xác thực', 'error');
            }
        } catch (error) {
            showAlert('Lỗi khi gửi mã xác thực', 'error');
        } finally {
            setLoading(false);
        }
    };

    const handleVerifyCode = async (e: any) => {
        e.preventDefault();
        setLoading(true);
        try {
            const res = await axios.post(
                `https://localhost:3031/api/account/verify-code?code=${code}&username=${username}`
            );

            if (res.status === 200) {
                showAlert('Xác thực thành công', 'success');
                setTurn(2);
            } else {
                showAlert('Xác thực code lỗi', 'error');
            }
        } catch (error) {
            showAlert('Xác thực code lỗi', 'error');
        } finally {
            setLoading(false);
        }
    };

    const handleChange = async (e: any) => {
        e.preventDefault();
        setLoading(true);
        try {
            const res = await axios.post(
                `https://localhost:3031/api/account/reset-password?newPass=${newPass}&username=${username}`
            );

            if (res.status === 200) {
                showAlert('Đổi mật khẩu thành công', 'success');

                setTurn(0);
                window.location.href = '/sign-in';
            } else {
                showAlert('Lỗi khi đổi mật khẩu', 'error');
            }
        } catch (error) {
            showAlert('Lỗi khi đổi mật khẩu', 'error');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="h-screen pb-20 max-w-screen-xl w-full flex justify-center items-center mx-auto gap-28">
            {AlertComponent}
            <div className="flex flex-col items-center w-1/2 border-2 border-[#d1efe5] rounded-3xl shadow-sm p-8">
                {turn === 0 && (
                    <>
                        <FontAwesomeIcon
                            icon={faKey}
                            className="w-12 h-12 border border-gray-500 p-4 rounded-md mb-4"
                            size="2x"
                        />
                        <h1 className="font-semibold text-2xl">
                            Quên mật khẩu ?
                        </h1>
                        <h2 className="font-medium text-lg mt-2 mb-6">
                            Đừng lo lắng, chúng tôi sẽ giúp bạn lấy lại mật khẩu
                        </h2>

                        <form
                            onSubmit={handleResetPass}
                            className="w-full mx-auto flex flex-col items-center"
                        >
                            <input
                                id="username"
                                name="username"
                                type="text"
                                autoComplete="username"
                                required
                                className="bg-gray-100 block w-[80%] p-4 rounded-lg shadow-md border-none focus:outline-none focus:ring focus:ring-[#2ebdc2] focus:border-[#2ebdc2] sm:text
                                        sm:leading-5 transition duration-300 ease-in-out text-[#101010] placeholder:text-gray-400"
                                placeholder="Tài khoản của bạn"
                                value={username}
                                onChange={(e) => setUsername(e.target.value)}
                            />

                            <input
                                id="email"
                                name="email"
                                type="email"
                                autoComplete="email"
                                required
                                className="mt-6 bg-gray-100 block w-[80%] p-4 rounded-lg shadow-md border-none focus:outline-none focus:ring focus:ring-[#2ebdc2] focus:border-[#2ebdc2] sm:text
                                        sm:leading-5 transition duration-300 ease-in-out text-[#101010] placeholder:text-gray-400"
                                placeholder="Email của bạn"
                                value={email}
                                onChange={(e) => setEmail(e.target.value)}
                            />

                            <button
                                type="submit"
                                className="w-[80%] mt-6 cursor-pointer flex items-center justify-center hover:bg-[#2ebdc2] text-white font-semibold py-3 rounded-lg shadow-sm transition duration-300 ease-in-out bg-[#0b0b0b]"
                            >
                                {loading ? 'Loading...' : 'Xác nhận'}
                            </button>
                        </form>
                    </>
                )}

                {turn === 1 && (
                    <>
                        <FontAwesomeIcon
                            icon={faMailBulk}
                            className="w-12 h-12 border border-gray-500 p-4 rounded-md mb-4"
                            size="2x"
                        />
                        <h1 className="font-semibold text-2xl">
                            Xác thực code
                        </h1>
                        <h2 className="font-medium text-lg mt-2 mb-6">
                            Vui lòng đăng nhập email để lấy code xác nhận
                        </h2>

                        <form
                            onSubmit={handleVerifyCode}
                            className="w-full mx-auto flex flex-col items-center"
                        >
                            <input
                                id="code"
                                name="code"
                                type="text"
                                autoComplete="code"
                                required
                                className="bg-gray-100 block w-[80%] p-4 rounded-lg shadow-md border-none focus:outline-none focus:ring focus:ring-[#2ebdc2] focus:border-[#2ebdc2] sm:text
                                        sm:leading-5 transition duration-300 ease-in-out text-[#101010] placeholder:text-gray-400"
                                placeholder="Nhập code tại đây"
                                value={code}
                                onChange={(e) => setCode(e.target.value)}
                            />

                            <button
                                type="submit"
                                className="w-[80%] mt-6 cursor-pointer flex items-center justify-center hover:bg-[#2ebdc2] text-white font-semibold py-3 rounded-lg shadow-sm transition duration-300 ease-in-out bg-[#0b0b0b]"
                            >
                                {loading ? 'Loading...' : 'Tiếp tục'}
                            </button>
                        </form>
                    </>
                )}

                {turn === 2 && (
                    <>
                        <FontAwesomeIcon
                            icon={faKey}
                            className="w-12 h-12 border border-gray-500 p-4 rounded-md mb-4"
                            size="2x"
                        />
                        <h1 className="font-semibold text-2xl">Đổi mật khẩu</h1>
                        <h2 className="font-medium text-lg mt-2 mb-6">
                            Hãy nhập mật khẩu mới
                        </h2>

                        <form
                            onSubmit={handleChange}
                            className="w-full mx-auto flex flex-col items-center"
                        >
                            <input
                                id="newPass"
                                name="newPass"
                                type="text"
                                autoComplete="newPass"
                                required
                                className="bg-gray-100 block w-[80%] p-4 rounded-lg shadow-md border-none focus:outline-none focus:ring focus:ring-[#2ebdc2] focus:border-[#2ebdc2] sm:text
                                        sm:leading-5 transition duration-300 ease-in-out text-[#101010] placeholder:text-gray-400"
                                placeholder="Nhập mật khẩu mới tại đây"
                                value={newPass}
                                onChange={(e) => setNewPass(e.target.value)}
                            />

                            <button
                                type="submit"
                                className="w-[80%] mt-6 cursor-pointer flex items-center justify-center hover:bg-[#2ebdc2] text-white font-semibold py-3 rounded-lg shadow-sm transition duration-300 ease-in-out bg-[#0b0b0b]"
                            >
                                {loading ? 'Loading...' : 'Xác nhận'}
                            </button>
                        </form>
                    </>
                )}

                <a href="/sign-in" className="hover:underline mt-4">
                    Quay trở lại đăng nhập
                </a>
            </div>
        </div>
    );
};

export default ForgotPassword;
