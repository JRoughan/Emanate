import React from 'react';
import { Route } from 'react-router';
import Layout from './components/Layout';
import Home from './components/Home';
import FetchData from './components/FetchData';
import Admin from './components/Admin';

export default () => (
    <Layout>
        <Route exact path='/' component={Home} />
        <Route path='/fetch-data/:startDateIndex?' component={FetchData} />
        <Route path='/admin' component={Admin} />
    </Layout>
);
