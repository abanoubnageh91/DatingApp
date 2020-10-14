import { Photo } from './photo';

export interface User {
    id: number;
    userName: string;
    knownAs: string;
    age: number;
    created: Date;
    gender: string;
    lastActive: Date;
    photoUrl: string;
    city: string;
    country: string;
    interests?: string;
    introduction?: string;
    lookingFor?: string;
    photos?: Photo[];
    roles?: string[];
}
