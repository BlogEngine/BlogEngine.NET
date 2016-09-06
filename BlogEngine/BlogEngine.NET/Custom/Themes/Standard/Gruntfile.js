module.exports = function (grunt) {
    grunt.initConfig({
        sass: {
            options: {
                noCache: true
            },
            dist: {
                options: {
                    style: 'compressed',
                },
                files: {
                    'src/css/styles.min.css': 'src/scss/styles.scss',
                }
            }
        },
        watch: {
            src: {
                files: ['src/scss/**/*.scss'],
                tasks: ['sass']
            }
        }
    });
    grunt.loadNpmTasks('grunt-contrib-sass');
    grunt.loadNpmTasks('grunt-contrib-watch');
    grunt.registerTask('default', ['watch']);
};
