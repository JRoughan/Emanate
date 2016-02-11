import uuid from 'node-uuid';
import alt from '../libs/alt';
import DelcomProfileActions from '../actions/DelcomProfileActions';

class DelcomProfileStore {

    constructor() {
        this.bindActions(DelcomProfileActions);

        this.profiles = [];
        this.profiles.concat();

        this.exportPublicMethods({
            getAllProfiles: this.getAllProfiles.bind(this),
            getDefaultProfile: this.getDefaultProfile.bind(this)
        });
    }

    create(profile) {
        const profiles = this.profiles;

        profiles.id = uuid.v4();

        this.setState({
            profiles: profiles.concat(profile)
        });

        return profile;
    }

    update(updatedProfile) {
        const profiles = this.profiles.map(profile => {
            if(profile.id === updatedProfile.id) {
                return Object.assign({}, profile, updatedProfile);
            }

            return profile;
        });

        this.setState({profiles});
    }

    delete(id) {
        this.setState({
            outputs: this.profiles.filter(profile => profile.id !== id)
        });
    }

    getAllProfiles() {
        const profiles = this.profiles;

        return profiles.map(function(profile) {
            return {
                id: profile.id,
                name: profile.name
            };
        });
    }

    getDefaultProfile() {
        return {
            id: '621d8942-2609-43df-acf3-b9f4339d4664',
            name: 'Default',
            hasRestrictedHours: false,
            states: [
                {
                    state: 'Unknown',
                    green: false,
                    yellow: false,
                    red: true,
                    flash: true,
                    buzzer: false
                },
                {
                    state: 'Succeeded',
                    green: true,
                    yellow: false,
                    red: false,
                    flash: false,
                    buzzer: false
                },
                {
                    state: 'Failed',
                    green: false,
                    yellow: false,
                    red: true,
                    flash: false,
                    buzzer: false
                },
                {
                    state: 'Error',
                    green: false,
                    yellow: false,
                    red: true,
                    flash: true,
                    buzzer: false
                },
                {
                    state: 'Running',
                    green: false,
                    yellow: true,
                    red: false,
                    flash: true,
                    buzzer: false
                }
            ]
        }
    }
}

export default alt.createStore(DelcomProfileStore, 'DelcomProfileStore');
