using CircularSeasManager.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms;

namespace CircularSeasManager.Models {
    public class AsistenteMaterialModel : BaseModel {

        //Almacén de todos los datos
        public InfoTopsis DataMaterial = new InfoTopsis();
        public List<Filament> filamentosfiltrados = new List<Filament>();
        //Colección con identificador de propiedad y valor de usuario de "importancia"
        public ObservableCollection<ValueUser> ValueUserCollection { get; set; }
        //Colección co resultado da avaliación TOPSIS
        public ObservableCollection<ResultadoTOPSIS> ResultadoTOPSISCollection { get; set; }
        //Colección cas features
        public ObservableCollection<FeaturesUser> FeaturesUserCollection { get; set; }
        //Comando intermedio para mostrar info (dado que no se puede bindear un command en listview
        public Command CmdInfo { get; set; }

        //Almacén del resultado seleccionado
        private ResultadoTOPSIS _materialSeleccionado;
        public ResultadoTOPSIS MaterialSeleccionado {
            get { return _materialSeleccionado; }
            set {
                if (_materialSeleccionado != value) {
                    _materialSeleccionado = value;
                    //Lanza comando
                    CmdInfo.Execute(_materialSeleccionado);
                    OnPropertyChanged();
                }
            }
        }
        //uohwefefhw


        //Confirmación de que existe un resultado calculado
        private bool _HayResultado = false;
        public bool HayResultado {
            get { return _HayResultado; }
            set { _HayResultado = value; OnPropertyChanged(); }
        }

        //Mensaje con información del material
        private string _InfoMaterial;
        public string InfoMaterial {
            get { return _InfoMaterial; }
            set {
                if (_InfoMaterial != value) {
                    _InfoMaterial = value;
                    OnPropertyChanged();
                }
            }
        }
        //Clase para contener datos de valoración
        public class ValueUser {
            public string Propiedade { get; set; }
            public double Valoracion { get; set; }
        }

        //Clase de resultado
        public class ResultadoTOPSIS {
            public string NomeMaterial { get; set; }
            public double Afinidade { get; set; }
            public double Afinidade100 { get; set; }
            public string Stock { get; set; }
        }

        public class FeaturesUser {
            public string feature { get; set; }
            public ObservableCollection<string> optionsFeature { get; set; }
            public string featureValueSelected { get; set; }
        }

        /// <summary> Devuelve índices de desempeño de los materiales dados</summary>
        /// <param name="criterio"> Matriz de valoración dos materiais</param>
        /// <param name="evaluacion"> Vector de avaliación do usuario</param>
        /// <returns>Vector de performance</returns>
        public static double[] TOPSIS(double[,] criterio, double[] evaluacion, bool[] impacto) {
            /* CRITERIO: Matriz de mxn (materiales x criterios) que incluye la valoración
             para el material i sobre el criterio j en cada termino Xij
               EVALUACION: vector de n elementos (criterios) que incluye la importancia
             * que el usuario solicita para los j criterios Wj*/

            //Determinar dimensiones de los parámetros
            int n_mat = criterio.GetLength(0);
            int n_crit = criterio.GetLength(1);
            int n_eval = evaluacion.Length;

            /*PASO 1: Normalización de la matriz de decisión y de la evaluación. Los valores
             pueden no estar definidos por el mismo dominio. Se normaliza como
            Nij = Xij/((Sumatorio,j=1 a m) de (Xij)^2))*/

            //Obtener denominador común a todos los elementos de cada columna (sumatorio)
            double[] den_normdecision = new double[n_crit];
            for (int j = 0; j < n_crit; j++) {
                double sumacuadrada = 0;
                for (int i = 0; i < n_mat; i++) {
                    sumacuadrada += Math.Pow(criterio[i, j], 2);
                }
                den_normdecision[j] = Math.Sqrt(sumacuadrada);
            }

            //matriz normalizada. para cada elemento Xij, se normaliza respecto de la columna (criterios).
            double[,] crit_norm = new double[n_mat, n_crit];
            for (int i = 0; i < n_mat; i++) {
                for (int j = 0; j < n_crit; j++) {
                    crit_norm[i, j] = criterio[i, j] / den_normdecision[j];
                }
            }

            //Normalización de decision, para que la suma de ponderaciones sea 1. Wnj = Wj/(sumatorio Wj)
            double sumaeval = 0;
            for (int i = 0; i < n_eval; i++) {
                sumaeval += evaluacion[i];
            }
            double[] eval_norm = new double[n_eval];
            for (int i = 0; i < n_eval; i++) {
                eval_norm[i] = evaluacion[i] / sumaeval;
            }

            /*PASO 2: COnstrucción de matriz de decisión normalizada ponderada. Se calculan mediante
             la expresion vij=WnjxNij*/
            double[,] crit_pond = new double[n_mat, n_crit];
            for (int i = 0; i < n_mat; i++) {
                for (int j = 0; j < n_crit; j++) {
                    crit_pond[i, j] = crit_norm[i, j] * eval_norm[j];
                }
            }

            /*PASO 3: Determinar mejor y peor solución. Si los criterios son de beneficio,
             * para cada criterio A+=(max Vij) y A-=(min Vij) si son de coste entonces
             * A+=(min Vij) y A-=(max Vij)*/
            double[] Amas = new double[n_crit];
            double[] Amenos = new double[n_crit];
            for (int j = 0; j < n_crit; j++) {
                double max = crit_pond[0, j]; //seleccionar primer elemento
                double min = crit_pond[0, j]; //Seleccionar primer elemento
                for (int i = 1; i < n_mat; i++) {
                    if (crit_pond[i, j] > max) {
                        max = crit_pond[i, j];
                    }
                    if (crit_pond[i, j] < min) {
                        min = crit_pond[i, j];
                    }
                }
                //Según el impacto, se hace positivo o negativo
                if (impacto[j]) {
                    Amas[j] = max;
                    Amenos[j] = min;
                }
                else {
                    Amas[j] = min;
                    Amenos[j] = max;
                }

            }

            /*PASO 4: Calcular medidas de distancia de cada Alternativa a la solución ideal positiva
             y la solución ideal negativa. D+=RAIZ((Sumatorio de j=1 hasta n(Vij-Aj+)^2)) y
             D+=RAIZ((Sumatorio de j=1 hasta n(Vij-Aj-)^2))*/
            double[] Dmas = new double[n_mat];
            double[] Dmenos = new double[n_mat];
            for (int i = 0; i < n_mat; i++) {
                double filamas = 0;
                double filamenos = 0;
                for (int j = 0; j < n_crit; j++) {
                    filamas += Math.Pow((crit_pond[i, j] - Amas[j]), 2);
                    filamenos += Math.Pow((crit_pond[i, j] - Amenos[j]), 2);
                }
                Dmas[i] = Math.Sqrt(filamas);
                Dmenos[i] = Math.Sqrt(filamenos);
            }

            /*PASO 5: Calcular performance como la proximidad relativa a la solución ideal. Se calcula 
             como Ri=di-/(di++di-). La Performance devuelve unos valores para cada alternativa que cuanto
            mayor es su valor, mejor es la alternativa atendiendo a los criterios dados.*/
            double[] perform = new double[n_mat];
            for (int i = 0; i < n_mat; i++) {
                perform[i] = Dmenos[i] / (Dmas[i] + Dmenos[i]);
            }

            return perform;
        }
    }

    //CLASE JSON para deserializar los materiales
    public class MaterialJSON {
        public Printer printer { get; set; }
        public string[] properties { get; set; }
        public Material[] materials { get; set; }
    }

    public class Material {
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public double[] propiedades { get; set; }
        public int stock { get; set; }
    }

    public class Printer {
        public string nombre { get; set; }
        public string[] profiles { get; set; }
    }

    public class InfoTopsis {
        public Printer printer { get; set; }
        public string[] featuresLabels { get; set; }
        public string[] propertiesLabels { get; set; }
        public bool[] positiveImpact { get; set; }
        public Filament[] filaments { get; set; }
    }
    public class Filament {
        public string name { get; set; }
        public string description { get; set; }
        public bool[] features { get; set; }
        public double[] properties { get; set; }
        public int stock { get; set; }
    }
}
