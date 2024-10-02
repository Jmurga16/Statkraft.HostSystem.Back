using Stakraft.HostSystem.Support.SoporteEnum;
using Stakraft.HostSystem.Support.SoporteUtil;

namespace Stakraft.HostSystem.Support.SoporteDto
{
    public class Respuesta<T>
    {
        public int Codigo { get; set; }
        public string Mensaje { get; set; }
        public string MensajeDev { get; set; }
        public T Dato { get; set; }

        public Respuesta<T> RespuestaCorrectaListar(T datos)
        {
            this.Codigo = (int)CodigosEnum.TipoRespuesta.Correcto;
            this.Mensaje = Mensajes.listadoCorrecto;
            this.Dato = datos;
            return this;
        }

        public Respuesta(T datos)
        {
            this.Codigo = (int)CodigosEnum.TipoRespuesta.Correcto;
            this.Mensaje = Mensajes.listadoCorrecto;
            this.Dato = datos;
        }
        public Respuesta()
        { }

        public Respuesta<T> RespuestaCorrectaActualizar(T datos)
        {
            this.Codigo = (int)CodigosEnum.TipoRespuesta.Correcto;
            this.Mensaje = Mensajes.actualizacionCorrecta;
            this.Dato = datos;
            return this;
        }

        public Respuesta<T> RespuestaCorrectaAgregar(T datos)
        {
            this.Codigo = (int)CodigosEnum.TipoRespuesta.Correcto;
            this.Mensaje = Mensajes.correcto;
            this.Dato = datos;
            return this;
        }

        public Respuesta<T> RespuestaCorrectaInactivar(T dato)
        {
            this.Codigo = (int)CodigosEnum.TipoRespuesta.Correcto;
            this.Mensaje = Mensajes.inactivar;
            this.Dato = dato;
            return this;
        }

        public Respuesta<T> RespuestaCorrectaLogin(T dato)
        {
            this.Codigo = (int)CodigosEnum.TipoRespuesta.Correcto;
            this.Mensaje = Mensajes.respuestaLogin;
            this.Dato = dato;
            return this;
        }

        public Respuesta<T> RespuestaOperacionCompletado(T dato)
        {
            this.Codigo = (int)CodigosEnum.TipoRespuesta.Correcto;
            this.Mensaje = Mensajes.correcto;
            this.Dato = dato;
            return this;
        }

        public Respuesta<T> RespuestaCorrecta(T dato, string mensaje)
        {
            this.Codigo = (int)CodigosEnum.TipoRespuesta.Correcto;
            this.Mensaje = mensaje;
            this.Dato = dato;
            return this;
        }
    }
}
