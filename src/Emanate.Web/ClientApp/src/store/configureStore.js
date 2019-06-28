import { applyMiddleware, combineReducers, compose, createStore } from 'redux';
import thunk from 'redux-thunk';
import { routerReducer, routerMiddleware } from 'react-router-redux';
import * as WeatherForecasts from './WeatherForecasts';
import * as Admin from './Admin';
import * as DisplayDeviceProfilePicker from './DisplayDeviceProfilePicker';
import * as SignalR from '@aspnet/signalr';

export default function configureStore(history, initialState) {
    const reducers = {
        weatherForecasts: WeatherForecasts.reducer,
        admin: Admin.reducer,
        displayDeviceProfilePicker: DisplayDeviceProfilePicker.reducer
    };

    const middleware = [
        thunk,
        routerMiddleware(history),
        signalRInvokeMiddleware
    ];

    // In development, use the browser's Redux dev tools extension if installed
    const enhancers = [];
    const isDevelopment = process.env.NODE_ENV === 'development';
    if (isDevelopment && typeof window !== 'undefined' && window.devToolsExtension) {
        enhancers.push(window.devToolsExtension());
    }

    const rootReducer = combineReducers({
        ...reducers,
        routing: routerReducer
    });

    const store = createStore(
        rootReducer,
        initialState,
        compose(applyMiddleware(...middleware), ...enhancers)
    );

    signalRRegisterCommands(store);

    return store;
}

const connection = new SignalR.HubConnectionBuilder()
    .withUrl("/counterx")
    .configureLogging(SignalR.LogLevel.Information)
    .build();

export function signalRInvokeMiddleware(store: any) {
    return (next: any) => async (action: any) => {
        switch (action.type) {
            case "ADD_DISPLAY_DEVICE":
                connection.invoke('AddDisplayDevice');
                break;
            case "REMOVE_DISPLAY_DEVICE":
                connection.invoke('RemoveDisplayDevice');
                break;
        }

        return next(action);
    }
}

export function signalRRegisterCommands(store: any, callback: Function) {

    connection.on('DisplayDeviceAdded', device => {
        store.dispatch({ type: 'DISPLAY_DEVICE_ADDED', newDisplayDevice: device });
    });

    connection.on('DisplayDeviceRemoved', id => {
        store.dispatch({ type: 'DISPLAY_DEVICE_REMOVED', oldDisplayDeviceId: id });
    });

    connection.on('DisplayDeviceUpdated', device => {
        store.dispatch({ type: 'DISPLAY_DEVICE_UPDATED', updatedDevice: device });
    });

    connection.on('DisplayDeviceProfileAdded', profile => {
        store.dispatch({ type: 'DISPLAY_DEVICE_PROFILE_ADDED', newDisplayDeviceProfile: profile });
    });

    connection.on('DisplayDeviceProfileRemoved', id => {
        store.dispatch({ type: 'DISPLAY_DEVICE_PROFILE_REMOVED', oldDisplayDeviceProfileId: id });
    });

    connection.start().then(function () {
        console.log("connected");
    });
}





