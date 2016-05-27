﻿using Autofac;

namespace Emanate.Core
{
    public interface IEmanateModule
    {
        void LoadServiceComponents(ContainerBuilder builder);
    }

    public interface IEmanateAdminModule
    {
        void LoadAdminComponents(ContainerBuilder builder);
    }

    public interface IModule
    {
        string Key { get; }
        string Name { get; }

        Direction Direction { get; }
    }

    public enum Direction
    {
        Unknown = 0,
        Input,
        Output
    }
}
