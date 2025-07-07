import type { GameWebSocket } from "../services/websocket";

// Game state types
export interface Player {
    id: string;
    deviceId: string;
    coins: number;
    rolls: number;
}

export interface ActivityLogEntry {
    timestamp: string;
    text: string;
}

// WebSocket message types
export interface LoginRequest {
    MessageType: "LoginRequest";
    DeviceId: string;
}

export interface LoginResponse {
    MessageType: "LoginResponse";
    PlayerId: string;
    DeviceId: string;
    InitialCoins: number;
    InitialRolls: number;
}

export interface UpdateResourcesRequest {
    MessageType: "UpdateResourcesRequest";
    ResourceType: number;
    ResourceValue: number;
}

export interface ResourceUpdateResponse {
    MessageType: "ResourceUpdateResponse";
    ResourceType: number;
    NewBalance: number;
}

export interface SendGiftRequest {
    MessageType: "SendGiftRequest";
    FriendPlayerId: string;
    ResourceType: number;
    ResourceValue: number;
}

export interface GiftEvent {
    MessageType: "GiftEvent";
    FromPlayerId: string;
    ResourceType: number;
    Amount: number;
}

export interface SuccessResponse {
    MessageType: "SuccessResponse";
    Message: string;
}

export interface ErrorResponse {
    MessageType: "ErrorResponse";
    ErrorCode: string;
    Message: string;
}

export type ServerResponse =
    | LoginResponse
    | ResourceUpdateResponse
    | GiftEvent
    | SuccessResponse
    | ErrorResponse;

export type ClientMessage =
    | LoginRequest
    | UpdateResourcesRequest
    | SendGiftRequest;

// Context types
export interface GameContextType {
    player: Player | null;
    connection: GameWebSocket | null;
    activityLog: ActivityLogEntry[];
    error: string | null;
    login: (deviceId: string) => Promise<boolean>;
    logout: () => void;
    updateResources: (resourceType: number, amount: number) => void;
    sendGift: (friendPlayerId: string, resourceType: number, amount: number) => void;
    addActivity: (text: string) => void;
    clearError: () => void;
}