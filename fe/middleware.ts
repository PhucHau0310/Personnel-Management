import { NextRequest, NextResponse } from 'next/server';
import { cookies } from 'next/headers';

export async function middleware(request: NextRequest) {
    console.log('Running middleware');

    const cookieStore = await cookies();
    const access_token = cookieStore.get('access-token')?.value;

    // console.log({ access_token });
    // console.log('All cookies: ', cookieStore.getAll());

    if (!access_token) {
        console.log('Not access token in cookie');
        return NextResponse.redirect(
            new URL('/sign-in', request.url).toString()
        );
    }
    return NextResponse.next();
}

export const config = {
    matcher: ['/', '/data', '/list', '/setting'],
};
