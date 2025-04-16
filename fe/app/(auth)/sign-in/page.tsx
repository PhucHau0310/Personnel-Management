'use client';

import AnimationLogin from '../../../public/AnimationLogin.json';
import Lottie from 'react-lottie-player';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faLock, faUser } from '@fortawesome/free-solid-svg-icons';
import { useState } from 'react';
import { useRouter } from 'nextjs-toploader/app';
import { setCookie } from 'cookies-next';
import useAlert from '@/hooks/useAlert';

const SignIn = () => {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [isFocusedUsername, setFocusedUsername] = useState(false);
    const [isFocusedPass, setFocusedPass] = useState(false);
    const [isLoading, setLoading] = useState(false);
    const router = useRouter();
    const { showAlert, AlertComponent } = useAlert();

    const handleLogin = async (e: any) => {
        e.preventDefault();
        setLoading(true);
        try {
            const data = {
                username,
                password,
            };

            const res = await fetch('https://localhost:3031/api/auth/login', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(data),
            });

            const result = await res.json();

            if (res.ok) {
                setCookie('access-token', result.data.accessToken);
                setCookie('refresh-token', result.data.refreshToken);

                setUsername('');
                setPassword('');

                showAlert('Đăng nhập thành công.', 'success');
                router.push('/');
            } else {
                showAlert(
                    'Tài khoản hoặc mật khẩu sai, vui lòng thử lại !',
                    'error'
                );
                setPassword('');
            }
        } catch (error) {
            console.log(error);
            showAlert('Lỗi trong quá trình đăng nhập ở máy chủ !', 'error');
        } finally {
            setLoading(false);
        }
    };

    const handleUsernameChange = (e: any) => {
        setUsername(e.target.value);
    };

    const handlePasswordChange = (e: any) => {
        setPassword(e.target.value);
    };

    return (
        <>
            {AlertComponent}
            <div className="h-screen max-w-screen-xl w-full flex justify-center items-center mx-auto gap-28">
                <div className="w-1/2 border-2 border-[#d1efe5] rounded-3xl shadow-sm p-8">
                    <h1 className="text-2xl font-semibold text-center text-[#0b0b0b]">
                        Đăng nhập
                    </h1>

                    <form
                        onSubmit={(e) => handleLogin(e)}
                        className="mt-8 space-y-6 w-[80%] mx-auto"
                    >
                        <div className="space-y-1 mb-8 relative">
                            <input
                                id="username"
                                name="username"
                                type="text"
                                autoComplete="username"
                                required
                                className="bg-gray-100 block w-full pl-12 pr-4 py-6 rounded-lg shadow-md border-none focus:outline-none focus:ring focus:ring-[#2ebdc2] focus:border-[#2ebdc2] sm:text
                                    sm:leading-5 transition duration-300 ease-in-out text-[#101010] placeholder:text-gray-400"
                                placeholder="Tài khoản"
                                onFocus={() => setFocusedUsername(true)}
                                onBlur={() => setFocusedUsername(false)}
                                value={username}
                                onChange={(e) => handleUsernameChange(e)}
                            />

                            <FontAwesomeIcon
                                icon={faUser}
                                className={`absolute left-4 translate(-1/2, -1/2) top-[28%] ${
                                    isFocusedUsername
                                        ? 'text-[#2ebdc2]'
                                        : 'text-gray-500'
                                }`}
                                size="lg"
                            />
                        </div>

                        <div className="space-y-1 relative">
                            <input
                                id="pass"
                                name="pass"
                                type="password"
                                autoComplete="pass"
                                required
                                className="bg-gray-100 block w-full pl-12 pr-4 py-6 border-none rounded-lg shadow-md focus:outline-none focus:ring focus:ring-[#2ebdc2] focus:border-[#2ebdc2] sm:text
                                    sm:leading-5 transition duration-300 ease-in-out text-[#101010] placeholder:text-gray-400"
                                placeholder="Mật khẩu"
                                onFocus={() => setFocusedPass(true)}
                                onBlur={() => setFocusedPass(false)}
                                value={password}
                                onChange={(e) => handlePasswordChange(e)}
                            />

                            <FontAwesomeIcon
                                icon={faLock}
                                className={`absolute left-4 translate(-1/2, -1/2) top-[28%] ${
                                    isFocusedPass
                                        ? 'text-[#2ebdc2]'
                                        : 'text-gray-500'
                                }`}
                                size="lg"
                            />
                        </div>

                        <a
                            href="/forgot-password"
                            className="flex justify-end text-[#0b0b0b] font-semibold hover:underline transition-opacity duration-300 ease-in-out"
                        >
                            Quên mật khẩu?
                        </a>

                        <button
                            type="submit"
                            className="w-full cursor-pointer flex items-center justify-center hover:bg-[#2ebdc2] text-white font-semibold py-3 rounded-lg shadow-sm transition duration-300 ease-in-out bg-[#0b0b0b]"
                        >
                            {isLoading ? 'Loading...' : 'Đăng nhập'}
                        </button>
                    </form>

                    <div className="mt-8 flex items-center justify-center text-[#0b0b0b]">
                        <span>Chào mừng bạn đã trở lại</span>
                    </div>
                </div>

                <div className="w-1/2">
                    <Lottie loop animationData={AnimationLogin} play />
                </div>
            </div>
        </>
    );
};

export default SignIn;
