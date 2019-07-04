export const actionCreators = {

    setDisplayDeviceProfile: (device, profileId) => async () => {
        const updatedDevice = {
            ...device,
            profileId: profileId
        }

        const url = 'api/DisplayDevices/' + updatedDevice.id;
        await fetch(url, {
            method: 'put',
            body: JSON.stringify(updatedDevice),
            headers: {
                "Content-Type": "application/json"
            }
        });
    }
};
