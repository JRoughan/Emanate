import { applyMiddleware, combineReducers, compose, createStore } from 'redux';
import thunk from 'redux-thunk';
import { routerReducer, routerMiddleware } from 'react-router-redux';
import * as SignalR from '@aspnet/signalr';

import * as Dashboard from './Dashboard';

import * as Admin from './Admin';
import * as DisplayDeviceGroup from './DisplayDeviceGroup';
import * as DisplayDevice from './DisplayDevice';
import * as NewDisplayDeviceDialog from './NewDisplayDeviceDialog';
import * as SourceDeviceGroup from './SourceDeviceGroup';
import * as SourceDevice from './SourceDevice';
import * as NewSourceDeviceDialog from './NewSourceDeviceDialog';

export default function configureStore(history, initialState) {
    const reducers = {
        dashboard: Dashboard.reducer,

        admin: Admin.reducer,
        displayDeviceGroup: DisplayDeviceGroup.reducer,
        displayDevice: DisplayDevice.reducer,
        newDisplayDeviceDialog: NewDisplayDeviceDialog.reducer,
        sourceDeviceGroup: SourceDeviceGroup.reducer,
        sourceDevice: SourceDevice.reducer,
        newSourceDeviceDialog: NewSourceDeviceDialog.reducer
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
    .withUrl("/notifications")
    .configureLogging(SignalR.LogLevel.Information)
    .build();

export function signalRInvokeMiddleware(store) {
    return (next) => async (action) => {
        switch (action.type) {
            case "ADD_DISPLAY_DEVICE":
                connection.invoke('AddDisplayDevice');
                break;
            case "REMOVE_DISPLAY_DEVICE":
                connection.invoke('RemoveDisplayDevice');
                break;
            default:
                break;
        }

        return next(action);
    }
}

export function signalRRegisterCommands(store, callback) {

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

    connection.on('DisplayDeviceProfileUpdated', profile => {
        store.dispatch({ type: 'DISPLAY_DEVICE_PROFILE_UPDATED', updatedDisplayDeviceProfile: profile });
    });

    connection.on('DisplayDeviceTypeAdded', profile => {
        store.dispatch({ type: 'DISPLAY_DEVICE_TYPE_ADDED', newDisplayDeviceProfile: profile });
    });

    connection.on('DisplayDeviceTypeRemoved', id => {
        store.dispatch({ type: 'DISPLAY_DEVICE_TYPE_REMOVED', oldDisplayDeviceProfileId: id });
    });

    connection.on('DisplayDeviceTypeUpdated', profile => {
        store.dispatch({ type: 'DISPLAY_DEVICE_TYPE_UPDATED', updatedDisplayDeviceProfile: profile });
    });

    connection.on('SourceDeviceAdded', device => {
        store.dispatch({ type: 'SOURCE_DEVICE_ADDED', newSourceDevice: device });
    });

    connection.on('SourceDeviceRemoved', id => {
        store.dispatch({ type: 'SOURCE_DEVICE_REMOVED', oldSourceDeviceId: id });
    });

    connection.on('SourceDeviceUpdated', device => {
        store.dispatch({ type: 'SOURCE_DEVICE_UPDATED', updatedDevice: device });
    });

    connection.on('SourceDeviceProfileAdded', profile => {
        store.dispatch({ type: 'SOURCE_DEVICE_PROFILE_ADDED', newSourceDeviceProfile: profile });
    });

    connection.on('SourceDeviceProfileRemoved', id => {
        store.dispatch({ type: 'SOURCE_DEVICE_PROFILE_REMOVED', oldSourceDeviceProfileId: id });
    });

    connection.on('SourceDeviceProfileUpdated', profile => {
        store.dispatch({ type: 'SOURCE_DEVICE_PROFILE_UPDATED', updatedSourceDeviceProfile: profile });
    });

    connection.on('SourceDeviceTypeAdded', profile => {
        store.dispatch({ type: 'SOURCE_DEVICE_TYPE_ADDED', newSourceDeviceProfile: profile });
    });

    connection.on('SourceDeviceTypeRemoved', id => {
        store.dispatch({ type: 'SOURCE_DEVICE_TYPE_REMOVED', oldSourceDeviceProfileId: id });
    });

    connection.on('SourceDeviceTypeUpdated', profile => {
        store.dispatch({ type: 'SOURCE_DEVICE_TYPE_UPDATED', updatedSourceDeviceProfile: profile });
    });

    connection.on('DisplayConfigurationAdded', configuration => {
        store.dispatch({ type: 'DISPLAY_CONFIGURATION_ADDED', newDisplayConfiguration: configuration });
    });

    connection.on('DisplayConfigurationRemoved', id => {
        store.dispatch({ type: 'DISPLAY_CONFIGURATION_REMOVED', oldDisplayConfigurationId: id });
    });

    connection.on('DisplayConfigurationUpdated', configuration => {
        store.dispatch({ type: 'DISPLAY_CONFIGURATION_UPDATED', updatedConfiguration: configuration });
    });

    connection.start().then(function () {
        console.log("connected");
    });
}