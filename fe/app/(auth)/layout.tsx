import type { Metadata } from 'next';

export const metadata: Metadata = {
    title: 'Authentication',
    description: 'Quản lý nhân viên',
};

const AuthLayout = ({ children }: { children: React.ReactNode }) => {
    return <div className="bg-[#f8ffff]">{children}</div>;
};

export default AuthLayout;
