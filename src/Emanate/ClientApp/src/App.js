import React from 'react';
import { Route } from 'react-router';
import Layout from './components/Layout';
import Dashboard from './components/Dashboard';
import Admin from './components/Admin';

export default () => (
    <Layout>
        <Route exact path='/' component={Dashboard} />
        <Route path='/admin' component={Admin} />
    </Layout>
);
