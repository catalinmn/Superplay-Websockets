export class GameWebSocket {
    private socket: WebSocket;
    private listeners: {
        message: ((data: any) => void)[];
        open: (() => void)[];
        close: (() => void)[];
        error: ((error: Event) => void)[];
    };

    constructor(url: string) {
        this.socket = new WebSocket(url);
        this.listeners = {
            message: [],
            open: [],
            close: [],
            error: []
        };

        this.socket.onmessage = (event) => {
            try {
                const data = JSON.parse(event.data);
                this.listeners.message.forEach(cb => cb(data));
            } catch (error) {
                console.error('Error parsing WebSocket message:', error);
            }
        };

        this.socket.onopen = () => {
            this.listeners.open.forEach(cb => cb());
        };

        this.socket.onclose = () => {
            this.listeners.close.forEach(cb => cb());
        };

        this.socket.onerror = (error) => {
            this.listeners.error.forEach(cb => cb(error));
        };
    }

    on(event: keyof typeof this.listeners, callback: any): void {
        this.listeners[event].push(callback);
    }

    send(message: any): boolean {
        if (this.socket.readyState === WebSocket.OPEN) {
            this.socket.send(JSON.stringify(message));
            return true;
        }
        return false;
    }

    close(): void {
        this.socket.close();
    }

    public get readyState(): number {
        return this.socket.readyState;
    }
}