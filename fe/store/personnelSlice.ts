import { createSlice, PayloadAction } from '@reduxjs/toolkit';

interface PersonnelState {
    id: string;
    username: string;
    numberId: string;
    classify: string;
    status: string;
    avatar: string;
    time: string;
    phoneNumber: string;
    gender: string;
    address: string;
    dateOfBirth: string;
}

const initialState: PersonnelState = {
    id: '',
    username: '',
    numberId: '',
    classify: '',
    status: '',
    avatar: '',
    time: '',
    phoneNumber: '',
    gender: '',
    address: '',
    dateOfBirth: '',
};

const personnelSlice = createSlice({
    name: 'personnel',
    initialState,
    reducers: {
        setPersonnelData: (state, action: PayloadAction<PersonnelState>) => {
            return { ...state, ...action.payload };
        },
        resetPersonnelData: () => initialState,
    },
});

export const { setPersonnelData, resetPersonnelData } = personnelSlice.actions;
export default personnelSlice.reducer;
